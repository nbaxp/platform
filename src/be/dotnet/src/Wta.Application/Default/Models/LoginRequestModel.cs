namespace Wta.Application.Default.Models;

public class LoginRequestModel : CaptchaModel
{
    [Required]
    public string? UserName { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public bool RememberMe { get; set; }
    public string? TenantNumber { get; set; }
}
