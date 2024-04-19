namespace Wta.Application.Default.Models;

public class RegisterModel : CaptchaModel
{
    [Required]
    [RegularExpression(@"\w{4,64}")]
    [Remote("IsUserNameAvailable", "User")]
    public string? UserName { get; set; }

    [Required]
    public string? Password { get; set; }

    [Compare("Password")]
    public string? ConfirmPassword { get; set; }

    [Required]
    [RegularExpression(@"^(1\d{10}|\w+@\w+\.\w+)$")]
    public string? EmailOrPhoneNumber { get; set; }
}
