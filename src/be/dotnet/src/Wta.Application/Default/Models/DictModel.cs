namespace Wta.Application.Default.Models;

[DependsOn<Dict>]
public class DictModel
{
    public Guid? Id { get; set; }
    public Guid? ParentId { get; set; }
    public string? Name { get; set; }
    public string? Number { get; set; }
}
