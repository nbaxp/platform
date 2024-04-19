namespace Wta.Application.Default.Models;

public class ForgotPasswordModel : CaptchaModel
{
    [Required]
    [RegularExpression(@"^(1\d{10}|\w+@\w+\.\w+)$")]
    public string? EmailOrPhoneNumber { get; set; }

    [Required]
    public string? Password { get; set; }

    [Compare("Password")]
    public string? ConfirmPassword { get; set; }
}
