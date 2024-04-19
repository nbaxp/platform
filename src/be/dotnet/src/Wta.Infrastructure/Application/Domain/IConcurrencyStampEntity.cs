namespace Wta.Infrastructure.Application.Domain;

public interface IConcurrencyStampEntity
{
    string ConcurrencyStamp { get; set; }
}
