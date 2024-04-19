namespace Wta.Infrastructure.Data;

public interface IDbContextManager
{
    void Add(DbContext dbContext);

    void BeginTransaction(bool autoSaveChanges = false);

    void CommitTransaction();

    void Rollback();
}
