using LootHub.Domain.Common;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace LootHub.Application.Common;

public sealed class QueryOptions<TEntity> where TEntity : BaseEntity
{
    public Expression<Func<TEntity, bool>>? Predicate { get; set; }
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; set; }
    public Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object?>>? Include { get; set; }
    public bool WithDeleted { get; set; } = false;
    public bool IsTracking { get; set; } = false;
}
