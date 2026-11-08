using LootHub.Domain.Common;
using System.Linq.Expressions;

namespace LootHub.Application.Common;

public interface IRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity?> GetAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetListAsync(QueryOptions<TEntity> options , CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}
