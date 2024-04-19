namespace Wta.Application.Default.Services;

public class AuthService(IRepository<User> repository, IHttpContextAccessor httpContextAccessor) : IAuthService
{
    [Authorize, Ignore]
    public bool HasPermission(string permission)
    {
        var normalizedUserName = httpContextAccessor.HttpContext?.User.Identity?.Name?.ToUpperInvariant()!;
        return repository.AsNoTracking()
            .Any(o => o.NormalizedUserName == normalizedUserName && o.UserRoles.Any(o => o.Role!.RolePermissions.Any(o => o.Permission!.Type == MenuType.Button && o.Permission!.Number == permission)));
    }
}
