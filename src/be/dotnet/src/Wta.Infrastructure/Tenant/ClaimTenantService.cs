using Microsoft.AspNetCore.Http;

namespace Wta.Infrastructure.Tenant;

[Service<ITenantService>(ServiceLifetime.Scoped)]
public class ClaimTenantService(IHttpContextAccessor httpContextAccessor) : ITenantService
{
    private string? _tenantNumber;

    public string? TenantNumber
    {
        get
        {
            if (_tenantNumber != null)
            {
                return _tenantNumber;
            }
            return httpContextAccessor!.HttpContext?.User.Claims.FirstOrDefault(o => o.Type == "TenantNumber")?.Value;
        }
        set
        {
            _tenantNumber = value;
        }
    }

    public List<string> Permissions { get; set; } = new List<string>();
}
