namespace Wta.Application.Default.Models;

[DependsOn<Department>]
public class DepartmentModel
{
    public Guid? Id { get; set; }
    public Guid? ParentId { get; set; }
    public string? Name { get; set; }
    public string? Number { get; set; }
}
