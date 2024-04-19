namespace Wta.Application.Default.Models;

public class ResetPasswordModel
{
    [Required]
    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(24, MinimumLength = 6)]
    public string? NewPassword { get; set; }

    [Compare(nameof(NewPassword))]
    [DataType(DataType.Password)]
    public string? ConfirmNewPassword { get; set; }
}
