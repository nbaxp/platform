namespace Wta.Infrastructure.Application.Domain;

public abstract class BaseTreeEntity<T> : BaseEntity, IOrderedEntity where T : BaseEntity
{
    public string Name { get; set; } = default!;

    [ReadOnly(true)]
    [RegularExpression(@"\w+")]
    public string Number { get; set; } = default!;

    public int Order { get; set; }
    public string Path { get; set; } = default!;

    public Guid? ParentId { get; set; }

    public T? Parent { get; set; }
    public List<T> Children { get; set; } = [];
}
