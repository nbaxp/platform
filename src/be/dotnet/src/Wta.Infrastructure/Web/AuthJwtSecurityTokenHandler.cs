namespace Wta.Infrastructure.Web;

public class AuthJwtSecurityTokenHandler(IServiceProvider serviceProvider) : JwtSecurityTokenHandler
{
    public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
    {
        var jwtHandler = serviceProvider.GetRequiredService<JsonWebTokenHandler>();
        var validationResult = jwtHandler.ValidateTokenAsync(token, validationParameters).Result;
        if (validationResult.IsValid)
        {
            validatedToken = validationResult.SecurityToken;
            return new AuthClaimsPrincipal(serviceProvider, new ClaimsPrincipal(validationResult.ClaimsIdentity));
        }
        throw validationResult.Exception;
    }
}
