namespace Wta.Application.Default.Domain;

[System, Display(Name = "部门", Order = 2)]
public class Department : BaseTreeEntity<Department>
{
    public List<User> Users { get; set; } = new List<User>();
}
