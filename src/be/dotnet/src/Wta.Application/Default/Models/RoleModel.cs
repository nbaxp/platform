namespace Wta.Application.Default.Models;

[DependsOn<Role>]
public class RoleModel
{
    [NotDefault]
    public Guid? Id { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Number { get; set; }

    public List<Guid> Permissions { get; set; } = new List<Guid>();
}
