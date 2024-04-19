namespace Wta.Application.Default.Controllers;

public class TokenController(ILogger<TokenController> logger,
    TokenValidationParameters tokenValidationParameters,
    SigningCredentials signingCredentials,
    JwtSecurityTokenHandler jwtSecurityTokenHandler,
    JwtOptions jwtOptions,
    IEncryptionService passwordHasher,
    IStringLocalizer stringLocalizer,
    IRepository<User> userRepository) : BaseController
{
    [HttpPost]
    [AllowAnonymous]
    public ApiResult<LoginResponseModel> Create(LoginRequestModel model)
    {
        userRepository.DisableTenantFilter();
        if (ModelState.IsValid)
        {
            var userQuery = userRepository.Query();
            var normalizedUserName = model.UserName?.ToUpperInvariant()!;
            var user = userQuery.FirstOrDefault(o => o.NormalizedUserName == normalizedUserName && o.TenantNumber == model.TenantNumber);
            if (user != null)
            {
                if (user.LockoutEnd.HasValue)
                {
                    if (user.LockoutEnd.Value >= DateTime.UtcNow)
                    {
                        var minutes = GetLeftMinutes(user);
                        throw new ProblemException(string.Format(CultureInfo.InvariantCulture, "用户已锁定,{0}分钟后解除", minutes));
                    }
                    else
                    {
                        user.LockoutEnd = null;
                        user.AccessFailedCount = 0;
                    }
                }

                if (user.PasswordHash != passwordHasher.HashPassword(model.Password!, user.SecurityStamp!))
                {
                    user.AccessFailedCount++;
                    if (user.AccessFailedCount >= jwtOptions.MaxFailedAccessAttempts)
                    {
                        user.LockoutEnd = DateTime.UtcNow.Add(jwtOptions.DefaultLockout);
                        user.AccessFailedCount = 0;
                        userRepository.SaveChanges();
                        var minutes = GetLeftMinutes(user);
                        throw new ProblemException(string.Format(CultureInfo.InvariantCulture, "用户已锁定,{0}分钟后解除", minutes));
                    }
                    else
                    {
                        userRepository.SaveChanges();
                        throw new ProblemException($"密码错误,剩余尝试错误次数为 {jwtOptions.MaxFailedAccessAttempts - user.AccessFailedCount}");
                    }
                }
                else
                {
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;
                    userRepository.SaveChanges();
                }
            }
            else
            {
                throw new ProblemException(stringLocalizer.GetString("用户名或密码错误"));
            }
            //
            var additionalClaims = new List<Claim>();
            if (user.TenantNumber != null)
            {
                additionalClaims.Add(new Claim("TenantNumber", user.TenantNumber));
            }
            var roles = userRepository.AsNoTracking()
                .Where(o => o.NormalizedUserName == normalizedUserName)
                .SelectMany(o => o.UserRoles)
                .Select(o => o.Role!.Number)
                .ToList()
                .Select(o => new Claim(tokenValidationParameters.RoleClaimType, o!));
            additionalClaims.AddRange(roles);
            var subject = CreateSubject(model.UserName!, additionalClaims);
            var result = new LoginResponseModel
            {
                AccessToken = CreateToken(subject, jwtOptions.AccessTokenExpires),
                RefreshToken = CreateToken(subject, model.RememberMe ? TimeSpan.FromDays(365) : jwtOptions.RefreshTokenExpires),
                ExpiresIn = (long)jwtOptions.AccessTokenExpires.TotalSeconds
            };
            return Json(result);
        }
        throw new BadRequestException();
    }

    [HttpPost]
    [AllowAnonymous]
    public ApiResult<LoginResponseModel> Refresh([FromBody] string refreshToken)
    {
        var validationResult = jwtSecurityTokenHandler.ValidateTokenAsync(refreshToken, tokenValidationParameters).Result;
        if (!validationResult.IsValid)
        {
            throw new ProblemException("RefreshToken验证失败", innerException: validationResult.Exception);
        }
        var subject = validationResult.ClaimsIdentity;
        var result = new LoginResponseModel
        {
            AccessToken = CreateToken(subject, jwtOptions.AccessTokenExpires),
            RefreshToken = CreateToken(subject, validationResult.SecurityToken.ValidTo.Subtract(validationResult.SecurityToken.ValidFrom)),
            ExpiresIn = (long)jwtOptions.AccessTokenExpires.TotalSeconds
        };
        return Json(result);
    }

    private ClaimsIdentity CreateSubject(string userName, List<Claim> additionalClaims)
    {
        var claims = new List<Claim>(additionalClaims) { new(tokenValidationParameters.NameClaimType, userName) };
        var subject = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        return subject;
    }

    private string CreateToken(ClaimsIdentity subject, TimeSpan timeout)
    {
        var now = DateTime.UtcNow;
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            // 签发者
            Issuer = tokenValidationParameters.ValidIssuer,
            // 接收方
            Audience = tokenValidationParameters.ValidAudience,
            // 凭据
            SigningCredentials = signingCredentials,
            // 声明
            Subject = subject,
            // 签发时间
            IssuedAt = now,
            // 生效时间
            NotBefore = now,
            // UTC 过期时间
            Expires = now.Add(timeout),
        };
        var securityToken = jwtSecurityTokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        var token = jwtSecurityTokenHandler.WriteToken(securityToken);
        return token;
    }

    private static string GetLeftMinutes(User user)
    {
        return (user.LockoutEnd!.Value - DateTime.UtcNow).TotalMinutes.ToString("f1", CultureInfo.InvariantCulture);
    }
}
