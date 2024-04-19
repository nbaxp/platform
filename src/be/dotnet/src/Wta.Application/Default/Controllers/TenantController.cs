using Mapster;
using Wta.Infrastructure.Application.Models;

namespace Wta.Application.Default.Controllers;

public class TenantController(ILogger<Tenant> logger,
    IStringLocalizer stringLocalizer,
    IRepository<Tenant> repository,
    IEventPublisher eventPublisher,
    IRepository<Role> roleRepository,
    IRepository<Permission> permissionRepository,
    IExportImportService exportImportService,
    IServiceProvider serviceProvider) : GenericController<Tenant, TenantModel>(logger, stringLocalizer, repository, eventPublisher, exportImportService)
{
    [Ignore]
    public override FileContentResult ImportTemplate()
    {
        return base.ImportTemplate();
    }

    [Ignore]
    public override ApiResult<bool> Import(ImportModel<TenantModel> model)
    {
        return base.Import(model);
    }

    [Ignore]
    public override FileContentResult Export(ExportModel<TenantModel> model)
    {
        return base.Export(model);
    }

    [Ignore]
    public override ApiResult<bool> Delete([FromBody] Guid[] items)
    {
        return base.Delete(items);
    }

    [AllowAnonymous]
    public override ApiResult<QueryModel<TenantModel>> Search(QueryModel<TenantModel> model)
    {
        return base.Search(model);
    }

    public override ApiResult<bool> Create(TenantModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new BadRequestException();
        }
        //创建租户
        var entity = new Tenant().FromModel(model, isCreate: true).SetIdBy(o => o.Number);
        Repository.Add(entity);
        Repository.SaveChanges();
        //初始化租户
        using var scope = serviceProvider.CreateScope();
        //设置租户Id
        var tenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        tenantService.TenantNumber = entity.Number;
        tenantService.Permissions = model.Permissions;
        //设置租户种子数据
        AppDomain.CurrentDomain.GetCustomerAssemblies()
        .SelectMany(o => o.GetTypes())
        .Where(o => o.IsClass && !o.IsAbstract && o.GetBaseClasses().Any(t => t == typeof(DbContext)))
        .OrderBy(o => o.GetCustomAttribute<DisplayAttribute>()?.Order ?? 0)
        .ForEach(dbContextType =>
        {
            var contextName = dbContextType.Name;
            if (scope.ServiceProvider.GetRequiredService(dbContextType) is DbContext dbContext)
            {
                var dbSeedType = typeof(IDbSeeder<>).MakeGenericType(dbContextType);
                scope.ServiceProvider.GetServices(dbSeedType).ForEach(o => dbSeedType.GetMethod(nameof(IDbSeeder<DbContext>.Seed))?.Invoke(o, [dbContext]));
            }
        });
        return Json(true);
    }

    public override ApiResult<TenantModel> Details([FromBody] Guid id)
    {
        var entity = Repository.AsNoTracking().FirstOrDefault(o => o.Id == id);
        if (entity == null)
        {
            throw new ProblemException("NotFound");
        }
        var model = entity.ToModel<Tenant, TenantModel>((entity, model) =>
        {
            roleRepository.DisableTenantFilter();
            var rolePermissions =
            model.Permissions = roleRepository.AsNoTracking()
            .Where(o => o.Number == "admin" && o.TenantNumber == entity.Number)
            .SelectMany(o => o.RolePermissions)
            .Select(o => o.Permission!.Number)
            .ToList();
        });
        return Json(model);
    }

    public override ApiResult<bool> Update([FromBody] TenantModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new BadRequestException();
        }
        var entity = Repository.Query().First(o => o.Id == model.Id);
        entity.FromModel(model);
        permissionRepository.DisableTenantFilter();
        var permissions = permissionRepository.Query().Where(o => o.TenantNumber == entity.Number).ToList();
        roleRepository.DisableTenantFilter();
        var tenantRole = roleRepository.Query()
            .Include(o => o.RolePermissions)
            .First(o => o.Number == "admin" && o.TenantNumber == entity.Number);
        var oldPermissions = permissions.Where(o => tenantRole.RolePermissions.Any(p => p.PermissionId == o.Id));
        var newPermissions = permissions.Where(o => model.Permissions.Any(p => o.Number == p));
        tenantRole.RolePermissions.RemoveAll(o => !newPermissions.Any(p => p.Id == o.PermissionId));
        var addList = newPermissions.Where(o => !tenantRole.RolePermissions.Any(p => p.PermissionId == o.Id)).Select(o => new RolePermission
        {
            RoleId = tenantRole.Id,
            PermissionId = o.Id,
        });
        tenantRole.RolePermissions.AddRange(addList);
        permissions.Where(o => !o.Disabled && !model.Permissions.Any(p => o.Number == p)).ForEach(o => o.Disabled = true);
        newPermissions.Where(o => o.Disabled && model.Permissions.Any(p => o.Number == p)).ForEach(o => o.Disabled = false);
        Repository.SaveChanges();
        return Json(true);
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> NoName([FromForm] string name)
    {
        return Json(!Repository.AsNoTracking().Any(o => o.Name == name));
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> NoNumber([FromForm] string number)
    {
        return Json(!Repository.AsNoTracking().Any(o => o.Number == number));
    }
}
