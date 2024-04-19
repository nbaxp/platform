namespace Wta.Infrastructure.Web;

public class AuthJwtBearerPostConfigureOptions(AuthJwtSecurityTokenHandler authJwtSecurityTokenHandler) : JwtBearerPostConfigureOptions, IPostConfigureOptions<JwtBearerOptions>
{
    [Obsolete]
    public new void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.SecurityTokenValidators.Clear();
        options.SecurityTokenValidators.Add(authJwtSecurityTokenHandler);
        base.PostConfigure(name, options);
    }
}
