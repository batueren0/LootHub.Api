using LootHub.Application.Common;
using LootHub.Application.DTOs.Order;

namespace LootHub.Application.Contracts.Services;

public interface IOrderService
{
    Task<Result<OrderDetailResponse>> CreateAsync(Guid userId, CreateOrderRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<OrderSummaryResponse>> GetMyOrdersAsync(Guid userId, CancellationToken ct = default);
    Task<Result<OrderDetailResponse>> GetByOrderNumberAsync(Guid userId, string orderNumber, CancellationToken ct = default);
}
