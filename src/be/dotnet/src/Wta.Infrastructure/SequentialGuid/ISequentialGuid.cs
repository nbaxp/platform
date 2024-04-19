namespace Wta.Infrastructure.SequentialGuid;

public interface ISequentialGuid
{
    Guid Create(string type);
}
