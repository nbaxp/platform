using Wta.Infrastructure.Application.Domain;

namespace Wta.Infrastructure.Data;

public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    public EfRepository(IServiceProvider serviceProvider)
    {
        var configType = serviceProvider.GetRequiredService(typeof(IEntityTypeConfiguration<>).MakeGenericType(typeof(TEntity)));
        var dbContextType = configType.GetType()
            .GetBaseClasses()
            .First(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(BaseDbConfig<>)).GenericTypeArguments.First();
        Context = (serviceProvider.GetRequiredService(dbContextType) as DbContext)!;
        DbSet = Context.Set<TEntity>();
    }

    protected DbContext Context { get; }
    protected DbSet<TEntity> DbSet { get; }

    public void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }

    public void AddRange(IEnumerable<TEntity> entities)
    {
        DbSet.AddRange(entities);
    }

    public IQueryable<TEntity> AsNoTracking()
    {
        return DbSet.AsNoTracking();
    }

    public void BeginTransaction()
    {
        Context.Database.BeginTransaction();
    }

    public void CommitTransaction()
    {
        Context.Database.CommitTransaction();
    }

    public void DisableSoftDeleteFilter()
    {
        Context.GetType().GetProperty("DisableSoftDeleteFilter")?.SetValue(Context, true);
    }

    public void DisableTenantFilter()
    {
        Context.GetType().GetProperty("DisableTenantFilter")?.SetValue(Context, true);
    }

    public Guid NewGuid()
    {
        return Context.NewGuid();
    }

    public IQueryable<TEntity> Query()
    {
        return DbSet;
    }

    public void Remove(TEntity entity)
    {
        DbSet.Remove(entity);
    }

    public int SaveChanges()
    {
        return Context.SaveChanges();
    }

    public Task<int> SaveChangesAsync()
    {
        return Context.SaveChangesAsync();
    }
}
