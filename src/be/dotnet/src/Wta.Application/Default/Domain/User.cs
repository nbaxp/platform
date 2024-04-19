namespace Wta.Application.Default.Domain;

[System, Display(Name = "用户", Order = 5)]
public class User : Entity
{
    public string? Name { get; set; }

    [ReadOnly(true)]
    public string UserName { get; set; } = default!;

    public string? Avatar { get; set; }
    public string NormalizedUserName { get; set; } = default!;
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PasswordHash { get; set; }
    public string SecurityStamp { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public int AccessFailedCount { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public bool LockoutEnabled { get; set; }
    public bool IsReadOnly { get; set; }
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public List<UserRole> UserRoles { get; set; } = [];
}
