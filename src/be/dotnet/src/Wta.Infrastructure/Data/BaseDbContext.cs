using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Wta.Infrastructure.Application.Domain;
using Wta.Infrastructure.Tenant;

namespace Wta.Infrastructure.Data;

public abstract class BaseDbContext<TDbContext> : DbContext where TDbContext : DbContext
{
    public static readonly ILoggerFactory DefaultLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

    private readonly string? _tenantNumber;

    public BaseDbContext(DbContextOptions<TDbContext> options, IServiceProvider serviceProvider) : base(options)
    {
        ServiceProvider = serviceProvider;
        DbContextManager = ServiceProvider.GetRequiredService<IDbContextManager>();
        DbContextManager.Add(this);
        _tenantNumber = ServiceProvider.GetService<ITenantService>()?.TenantNumber;
    }

    public IDbContextManager DbContextManager { get; }
    public bool DisableSoftDeleteFilter { get; set; }
    public bool DisableTenantFilter { get; set; }
    public IServiceProvider ServiceProvider { get; }

    public void CreateQueryFilter<TEntity>(ModelBuilder builder) where TEntity : BaseEntity
    {
        builder.Entity<TEntity>().HasQueryFilter(o =>
        (DisableSoftDeleteFilter == true || !o.IsDeleted) &&
        (DisableTenantFilter == true || o.TenantNumber == _tenantNumber));
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var entries = GetEntries();
        BeforeSave(entries);
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        var entries = GetEntries();
        BeforeSave(entries);
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected virtual void BeforeSave(List<EntityEntry> entries)
    {
        var userName = this.GetService<IHttpContextAccessor>().HttpContext?.User.Identity?.Name ?? "admin";
        var now = DateTime.UtcNow;
        foreach (var item in entries.Where(o => o.State == EntityState.Added || o.State == EntityState.Modified || o.State == EntityState.Deleted))
        {
            //实体IsReadOnly属性为true的不可删除
            if (item.State == EntityState.Deleted)
            {
                var isReadOnly = item.Entity.GetType().GetProperty("IsReadOnly")?.GetValue(item.Entity) as bool?;
                if (isReadOnly.HasValue && isReadOnly.Value)
                {
                    item.State = EntityState.Unchanged;
                }
            }
            //设置审计属性户
            if (item.Entity is BaseEntity entity)
            {
                //设置租户
                if (item.State == EntityState.Added)
                {
                    if (_tenantNumber != null)
                    {
                        entity.TenantNumber = _tenantNumber;
                    }
                }
                //第一次删除为伪删除
                if (item.State == EntityState.Deleted)
                {
                    if (!entity.IsDeleted)
                    {
                        item.State = EntityState.Modified;
                        entity.IsDeleted = true;
                    }
                }
                if (item.State == EntityState.Added)
                {
                    entity.CreatedOn = now;
                    entity.CreatedBy = userName;
                }
                else if (item.State == EntityState.Modified)
                {
                    entity.UpdatedOn = now;
                    entity.UpdatedBy = userName;
                }
                //设置行版本号
                if (item.State == EntityState.Added || item.State == EntityState.Modified)
                {
                    if (entity is IConcurrencyStampEntity concurrency)
                    {
                        concurrency.ConcurrencyStamp = Guid.NewGuid().ToString("N");
                    }
                }
            }
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(DefaultLoggerFactory);
        if (ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        optionsBuilder.EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var method = typeof(ModelBuilder).GetMethods().First(o => o.Name == nameof(modelBuilder.ApplyConfiguration));
        AppDomain.CurrentDomain.GetCustomerAssemblies()
            .SelectMany(o => o.GetTypes())
            .Where(o => o.IsClass && !o.IsAbstract && o.IsAssignableTo(typeof(BaseDbConfig<>).MakeGenericType(this.GetType())))
            .ForEach(configType =>
            {
                configType.GetInterfaces().Where(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)).ForEach(o =>
                {
                    var entityType = o.GenericTypeArguments.First();
                    //配置实体
                    var entityModlerBuilder = modelBuilder.Entity(entityType);
                    if (entityType.IsAssignableTo(typeof(BaseEntity)))
                    {
                        //配置软删除、多租户的全局过滤器
                        GetType().GetMethod(nameof(this.CreateQueryFilter))?.MakeGenericMethod(entityType).Invoke(this, [modelBuilder]);
                        //配置实体Id
                        entityModlerBuilder.HasKey(nameof(BaseEntity.Id));
                        entityModlerBuilder.Property(nameof(BaseEntity.Id)).ValueGeneratedNever();
                        //配置实体行版本号
                        if (entityType.IsAssignableTo(typeof(IConcurrencyStampEntity)))
                        {
                            entityModlerBuilder.Property(nameof(IConcurrencyStampEntity.ConcurrencyStamp)).ValueGeneratedNever();
                        }
                        //配置树形结构实体
                        if (entityType.IsAssignableTo(typeof(BaseTreeEntity<>).MakeGenericType(entityType)))
                        {
                            entityModlerBuilder.Property(nameof(BaseTreeEntity<BaseEntity>.Name)).IsRequired();
                            entityModlerBuilder.Property(nameof(BaseTreeEntity<BaseEntity>.Number)).IsRequired();
                            entityModlerBuilder.HasIndex(nameof(BaseTreeEntity<BaseEntity>.TenantNumber), nameof(BaseTreeEntity<BaseEntity>.Number)).IsUnique();
                            entityModlerBuilder.HasOne(nameof(BaseTreeEntity<BaseEntity>.Parent)).WithMany(nameof(BaseTreeEntity<BaseEntity>.Children)).HasForeignKey(nameof(BaseTreeEntity<BaseEntity>.ParentId)).OnDelete(DeleteBehavior.SetNull);
                        }
                    }
                    //配置属性
                    var properties = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                    properties.ForEach(prop =>
                    {
                        //配置只读字段（创建后不可更新）
                        if (prop.GetCustomAttributes<ReadOnlyAttribute>().Any())
                        {
                            entityModlerBuilder.Property(prop.Name).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                        }
                        //值对象非空
                        if (prop.PropertyType.IsValueType && !prop.PropertyType.IsNullableType())
                        {
                            entityModlerBuilder.Property(prop.Name).IsRequired();
                        }
                        //配置枚举存储为字符串
                        if (prop.PropertyType.GetUnderlyingType().IsEnum)
                        {
                            entityModlerBuilder.Property(prop.Name).HasConversion<string>();
                        }
                        //配置日期存取时为UTC时间
                        if (prop.PropertyType.GetUnderlyingType() == typeof(DateTime))
                        {
                            if (prop.PropertyType.IsNullableType())
                            {
                                entityModlerBuilder.Property<DateTime?>(prop.Name).HasConversion(v =>
                                v.HasValue ? v.Value.Kind == DateTimeKind.Utc ? v : v.Value.ToUniversalTime() : null,
                                v => v == null ? null : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));
                            }
                            else
                            {
                                entityModlerBuilder.Property<DateTime>(prop.Name).HasConversion(v =>
                                v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
                            }
                        }
                    });
                    //自定义配置
                    var config = ServiceProvider.GetRequiredService(typeof(IEntityTypeConfiguration<>).MakeGenericType(entityType));
                    method.MakeGenericMethod(entityType).Invoke(modelBuilder, [config]);
                });
            });
    }

    private List<EntityEntry> GetEntries()
    {
        ChangeTracker.DetectChanges();
        var entries = ChangeTracker.Entries().ToList();
        var eventPublisher = ServiceProvider.GetRequiredService<IEventPublisher>();
        var method = eventPublisher.GetType().GetMethod(nameof(eventPublisher.Publish))!;
        entries.ForEach(o =>
        {
            if (o.State == EntityState.Modified)
            {
                var type = typeof(EntityUpdatedEvent<>).MakeGenericType(o.Entity.GetType());
                var @event = Activator.CreateInstance(type, o.Entity);
                method.MakeGenericMethod(type).Invoke(eventPublisher, [@event]);
            }
        });
        return entries;
    }
}
