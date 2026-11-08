using LootHub.Application.Common;
using LootHub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LootHub.Infrastructure.Common;

public class BaseRepository<TEntity, TContext>(TContext context) : IRepository<TEntity>
    where TEntity : BaseEntity
    where TContext : DbContext
{
    protected DbSet<TEntity> Table => context.Set<TEntity>();

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(entity, cancellationToken);
        return entity;
    }
    public void Update(TEntity entity)
    {
        entity.LastModifiedAtUtc = TimeProvider.System.GetUtcNow();
        if (context.Entry(entity).State == EntityState.Detached)
            context.Update(entity);


    }
    public void Delete(TEntity entity)
    {
        var time = TimeProvider.System.GetUtcNow();
        entity.LastModifiedAtUtc = time;
        entity.DeletedAtUtc = time;
        entity.IsActive = false;
        if (context.Entry(entity).State == EntityState.Detached)
            context.Update(entity);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, bool withDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = Table.AsQueryable();
        if (withDeleted)
            query = query.IgnoreQueryFilters();
        if (predicate is not null)
            query = query.Where(predicate);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken = default)
    {
        var query = Table.AsQueryable();
        if (options.WithDeleted)
            query = query.IgnoreQueryFilters();
        if (options.Include is not null)
            query = options.Include(query);
        if (options.Predicate is not null)
            query = query.Where(options.Predicate);
        if (!options.IsTracking)
            query = query.AsNoTracking();

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListAsync(QueryOptions<TEntity> options, CancellationToken cancellationToken = default)
    {
        var query = Table.AsQueryable();
        if (options.WithDeleted)
            query = query.IgnoreQueryFilters();
        if (options.Include is not null)
            query = options.Include(query);
        if (options.Predicate is not null)
            query = query.Where(options.Predicate);
        if (options.OrderBy is not null)
            query = options.OrderBy(query);
        if (!options.IsTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }
}
