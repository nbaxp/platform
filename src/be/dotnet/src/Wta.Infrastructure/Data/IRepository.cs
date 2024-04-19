using Wta.Infrastructure.Application.Domain;

namespace Wta.Infrastructure.Data;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Guid NewGuid();

    void Add(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);

    void BeginTransaction();

    void CommitTransaction();

    IQueryable<TEntity> Query();

    IQueryable<TEntity> AsNoTracking();

    void Remove(TEntity entity);

    int SaveChanges();

    Task<int> SaveChangesAsync();

    void DisableSoftDeleteFilter();

    void DisableTenantFilter();
}
