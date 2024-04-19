namespace Wta.Infrastructure.Auth;

public interface IAuthService
{
    bool HasPermission(string permission);
}
