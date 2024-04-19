using Microsoft.EntityFrameworkCore.Storage;

namespace Wta.Infrastructure.Data;

[Service<IDbContextManager>(ServiceLifetime.Scoped)]
public class DbContextManager : IDbContextManager
{
    private bool _autoSaveChanges;

    private readonly List<DbContext> _dbContexts = new();
    private readonly Dictionary<DbContext, IDbContextTransaction> _transactions = new();
    private readonly List<IDbContextTransaction> _commitTransactions = new();

    public void Add(DbContext dbContext)
    {
        _dbContexts.Add(dbContext);
    }

    public void BeginTransaction(bool autoSaveChanges = false)
    {
        _autoSaveChanges = autoSaveChanges;
        foreach (var item in _dbContexts)
        {
            _transactions.Add(item, item.Database.BeginTransaction());
        }
    }

    public void CommitTransaction()
    {
        try
        {
            foreach (var transaction in _transactions)
            {
                if (_autoSaveChanges)
                {
                    transaction.Key.SaveChanges();
                }
                transaction.Value.Commit();
                _commitTransactions.Add(transaction.Value);
            }
        }
        catch
        {
            Rollback();

            throw;
        }
    }

    public void Rollback()
    {
        foreach (var transaction in _commitTransactions)
        {
            transaction.Rollback();
        }
    }
}
