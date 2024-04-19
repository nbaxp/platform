namespace Wta.Infrastructure.Application.Domain;

public abstract class BaseEntity : IResource
{
    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
    public string? TenantNumber { get; set; }
}
