namespace Wta.Application.Default.Controllers;

[Service<IAuthService>(ServiceLifetime.Transient)]
public class UserController(ILogger<User> logger,
    IStringLocalizer stringLocalizer,
    IRepository<User> repository,
    IEventPublisher eventPublisher,
    IExportImportService exportImportService,
    IHttpContextAccessor httpContextAccessor,
    IEncryptionService encryptionService) : GenericController<User, UserModel>(logger, stringLocalizer, repository, eventPublisher, exportImportService), IAuthService
{
    [Authorize, Ignore]
    public bool HasPermission(string permission)
    {
        var normalizedUserName = httpContextAccessor.HttpContext?.User.Identity?.Name?.ToUpperInvariant()!;
        return Repository.AsNoTracking()
            .Any(o => o.NormalizedUserName == normalizedUserName && o.UserRoles.Any(o => o.Role!.RolePermissions.Any(o => o.Permission!.Type == MenuType.Button && o.Permission!.Number == permission)));
    }

    [Authorize, Ignore]
    public ApiResult<UserInfoModel> Info()
    {
        var normalizedUserName = User.Identity?.Name?.ToUpperInvariant()!;
        var result = Repository
           .AsNoTracking()
           .Include(o => o.UserRoles)
           .ThenInclude(o => o.Role)
           .ThenInclude(o => o!.RolePermissions)
           .ThenInclude(o => o.Permission)
           .FirstOrDefault(o => o.NormalizedUserName == normalizedUserName)!;

        return Json(new UserInfoModel
        {
            UserName = result.UserName,
            Name = result.Name,
            Avatar = result.Avatar,
            Roles = result.UserRoles.Select(o => o.Role!.Number).ToList(),
            Permissions = result.UserRoles.SelectMany(o => o.Role!.RolePermissions).Select(o => o.Permission!.Number).ToList()
        });
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            var values = encryptionService.DecryptText(model.CodeHash!).Split(',');
            if (values[2] == model.EmailOrPhoneNumber)
            {
                var user = new User();
                user.FromModel(model);
                user.NormalizedUserName = user.UserName.ToUpperInvariant();
                user.SecurityStamp = encryptionService.CreateSalt();
                user.PasswordHash = encryptionService.HashPassword(model.Password!, user.SecurityStamp);
                var isEmail = Regex.IsMatch(model.EmailOrPhoneNumber, @"\w+@\w+\.\w+");
                if (isEmail)
                {
                    user.Email = model.EmailOrPhoneNumber;
                    user.NormalizedEmail = model.EmailOrPhoneNumber.ToUpperInvariant();
                    user.EmailConfirmed = true;
                }
                else
                {
                    user.PhoneNumber = model.EmailOrPhoneNumber;
                    user.PhoneNumberConfirmed = true;
                }
                Repository.Add(user);
                Repository.SaveChanges();
                return Json(true);
            }
            ModelState.AddModelError(nameof(model.EmailOrPhoneNumber), "EmailOrPhoneNumberNotMatch");
        }
        throw new BadRequestException();
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> ForgotPassword(ForgotPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var values = encryptionService.DecryptText(model.CodeHash!).Split(',');
            if (values[2] == model.EmailOrPhoneNumber)
            {
                var user = Repository.Query().FirstOrDefault(o => o.Email == model.EmailOrPhoneNumber || o.PhoneNumber == model.EmailOrPhoneNumber);
                if (user != null)
                {
                    user.PasswordHash = encryptionService.HashPassword(model.Password!, user.SecurityStamp!);
                    Repository.SaveChanges();
                    return Json(true);
                }
                ModelState.AddModelError(nameof(model.EmailOrPhoneNumber), "EmailOrPhoneNumberNotExist");
            }
            ModelState.AddModelError(nameof(model.EmailOrPhoneNumber), "EmailOrPhoneNumberNotMatch");
        }
        throw new BadRequestException();
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> ResetPassword(ResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var normalizedUserName = User.Identity?.Name?.ToUpperInvariant()!;
            var user = Repository.Query().FirstOrDefault(o => o.NormalizedUserName == normalizedUserName)!;
            if (user != null)
            {
                if (user.PasswordHash != encryptionService.HashPassword(model.CurrentPassword!, user.SecurityStamp!))
                {
                    ModelState.AddModelError(nameof(model.CurrentPassword), "WrongPassword");
                }
                else
                {
                    user.PasswordHash = encryptionService.HashPassword(model.NewPassword!, user.SecurityStamp!);
                    Repository.SaveChanges();
                    return Json(true);
                }
            }
            else
            {
                throw new ProblemException("UserNotExist");
            }
        }
        throw new BadRequestException();
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> HasUser([FromForm] string userName)
    {
        var normalizedUserName = userName.ToUpperInvariant();
        return Json(Repository.AsNoTracking().Any(o => o.NormalizedUserName == normalizedUserName));
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> NoUser([FromForm] string userName)
    {
        var normalizedUserName = userName.ToUpperInvariant();
        return Json(!Repository.AsNoTracking().Any(o => o.NormalizedUserName == normalizedUserName));
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> HasEmailOrPhoneNumber([FromForm] string emailOrPhoneNumber)
    {
        return Json(HasEmailOrPhoneNumberInternal(emailOrPhoneNumber));
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> NoEmailOrPhoneNumber([FromForm] string emailOrPhoneNumber)
    {
        return Json(!HasEmailOrPhoneNumberInternal(emailOrPhoneNumber));
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> NoEmail([FromForm] string email)
    {
        return Json(!HasEmailOrPhoneNumberInternal(email));
    }

    [AllowAnonymous, Ignore]
    public ApiResult<bool> NoPhoneNumber([FromForm] string phoneNumber)
    {
        return Json(!HasEmailOrPhoneNumberInternal(phoneNumber));
    }

    private bool HasEmailOrPhoneNumberInternal(string emailOrPhoneNumber)
    {
        var normalizedEmailOrPhoneNumber = emailOrPhoneNumber.ToUpperInvariant();
        var isEmail = Regex.IsMatch(emailOrPhoneNumber, @"\w+@\w+\.\w+");
        var query = Repository.AsNoTracking();
        var result = isEmail ? query.Any(o => o.NormalizedEmail == normalizedEmailOrPhoneNumber) : query.Any(o => o.PhoneNumber == emailOrPhoneNumber);
        return result;
    }

    protected override void ToModel(User entity, UserModel model)
    {
        model.Roles = entity.UserRoles.Select(o => o.RoleId).ToList();
    }

    protected override void ToEntity(User entity, UserModel model, bool isCreate)
    {
        if (isCreate && string.IsNullOrEmpty(model.Password))
        {
            var key = nameof(model.Password);
            ModelState.AddModelError(key, StringLocalizer.GetString("Required", StringLocalizer.GetString(key)));
            throw new BadRequestException();
        }
        if (string.IsNullOrEmpty(entity.NormalizedUserName) && !string.IsNullOrEmpty(entity.UserName))
        {
            entity.NormalizedUserName = entity.UserName.ToUpperInvariant();
        }
        if (string.IsNullOrEmpty(entity.NormalizedEmail) && !string.IsNullOrEmpty(entity.Email))
        {
            entity.NormalizedEmail = entity.Email!.ToUpperInvariant();
        }
        if (string.IsNullOrEmpty(entity.SecurityStamp))
        {
            entity.SecurityStamp = encryptionService.CreateSalt();
        }
        if (!string.IsNullOrEmpty(model.Password))
        {
            entity.PasswordHash = encryptionService.HashPassword(model.Password, entity.SecurityStamp);
        }
        entity.UserRoles.RemoveAll(o => !model.Roles.Contains(o.RoleId));
        entity.UserRoles.AddRange(model.Roles.Where(o => !entity.UserRoles.Any(p => p.RoleId == o)).Select(o => new UserRole { RoleId = o }));
    }
}
