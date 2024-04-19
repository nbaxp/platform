namespace Wta.Application.Default.Models;

public class UserInfoModel
{
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Avatar { get; set; }
    public List<string> Roles { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
}
