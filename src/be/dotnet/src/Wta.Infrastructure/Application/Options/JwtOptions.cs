namespace Wta.Infrastructure.Application.Configuration;

public class JwtOptions
{
    public string Issuer { get; set; } = "Issuer";
    public string Audience { get; set; } = "Audience";
    public string IssuerSigningKey { get; set; } = "12345678123456781234567812345678";

    public int MaxFailedAccessAttempts { get; set; } = 5;
    public TimeSpan AccessTokenExpires { get; set; } = TimeSpan.FromMinutes(10);
    public TimeSpan RefreshTokenExpires { get; set; } = TimeSpan.FromHours(24);
    public TimeSpan DefaultLockout { get; set; } = TimeSpan.FromMinutes(10);
}
