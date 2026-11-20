using LootHub.Application.Common;
using LootHub.Application.Contracts.Repositories;
using LootHub.Application.DTOs.Order;
using LootHub.Domain.Entities;
using LootHub.Infrastructure.Common;
using LootHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LootHub.Infrastructure.Repositories;

public class OrderRepository(AppDbContext context) : BaseRepository<Order, AppDbContext>(context), IOrderRepository
{
    public async Task<PagedResult<Order>> GetPagedAsync(AdminOrderQuery query, CancellationToken ct = default)
    {
        var q = Table.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(o => o.OrderNumber.Contains(query.Search) || o.CustomerName.Contains(query.Search));
        if (query.Status is not null)
            q = q.Where(o => o.Status == query.Status);

        var totalCount = await q.CountAsync(ct);

        q = q.OrderByDescending(o => o.CreatedAtUtc).ThenBy(o => o.Id);

        var items = await q
            .Include(o => o.Items)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Order>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
        };
    }
}
