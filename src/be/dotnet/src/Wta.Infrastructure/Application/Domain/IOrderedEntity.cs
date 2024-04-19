namespace Wta.Infrastructure.Application.Domain;

public interface IOrderedEntity
{
    int Order { get; set; }
}
