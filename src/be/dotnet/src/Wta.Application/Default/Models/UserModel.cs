namespace Wta.Application.Default.Models;

public class UserModel
{
    public Guid? Id { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public string? Avatar { get; set; }
    public Guid? DepartmentId { get; set; }
    public List<Guid> Roles { get; set; } = [];
}
