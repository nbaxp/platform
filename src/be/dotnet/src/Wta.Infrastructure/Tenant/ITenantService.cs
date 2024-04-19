namespace Wta.Infrastructure.Tenant;

public interface ITenantService
{
    string? TenantNumber { get; set; }
    List<string> Permissions { get; set; }
}
