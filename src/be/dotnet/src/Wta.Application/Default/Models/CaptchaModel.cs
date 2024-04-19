using Wta.Infrastructure;

namespace Wta.Application.Default.Models;

public class CaptchaModel : IValidatableObject
{
    [Required]
    public string? AuthCode { get; set; }

    [Required, Hidden]
    public string? CodeHash { get; set; }

    [Hidden]
    public DateTime? Expires { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        using var scope = WtaApplication.Application.Services.CreateScope();
        var stringLocalizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer>();
        var encryptionService = scope.ServiceProvider.GetRequiredService<IEncryptionService>();
        var values = encryptionService.DecryptText(CodeHash!).Split(',');
        var timeout = DateTime.Parse(values[0], CultureInfo.InvariantCulture);
        var code = values[1];
        if (DateTime.UtcNow > timeout)
        {
            yield return new ValidationResult(stringLocalizer["CaptchaErrorTimeout"], [nameof(AuthCode)]);
        }
        if (code != AuthCode)
        {
            yield return new ValidationResult(stringLocalizer["CaptchaError"], [nameof(AuthCode)]);
        }
    }
}
