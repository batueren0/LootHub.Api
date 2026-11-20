using LootHub.Application.Common;
using LootHub.Application.DTOs.Order;
using LootHub.Domain.Entities;

namespace LootHub.Application.Contracts.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<PagedResult<Order>> GetPagedAsync(AdminOrderQuery query, CancellationToken ct = default);
}
