namespace Wta.Application.Default.Controllers;

public class RoleController(ILogger<Role> logger, IStringLocalizer stringLocalizer, IRepository<Role> repository, IEventPublisher eventPublisher, IExportImportService exportImportService) : GenericController<Role, RoleModel>(logger, stringLocalizer, repository, eventPublisher, exportImportService)
{
    protected override void ToModel(Role entity, RoleModel model)
    {
        model.Permissions = entity.RolePermissions.Select(o => o.PermissionId).ToList();
    }
    protected override void ToEntity(Role entity, RoleModel model, bool isCreate)
    {
        entity.RolePermissions.RemoveAll(o => !model.Permissions.Contains(o.PermissionId));
        entity.RolePermissions.AddRange(model.Permissions.Where(o => !entity.RolePermissions.Any(p => p.PermissionId == o)).Select(o => new RolePermission { PermissionId = o }));
    }
}
