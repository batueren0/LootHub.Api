using LootHub.Application.Common;
using LootHub.Application.DTOs.Order;

namespace LootHub.Application.Contracts.Services;

public interface IOrderService
{
    Task<Result<OrderDetailResponse>> CreateAsync(Guid userId, CreateOrderRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<OrderSummaryResponse>> GetMyOrdersAsync(Guid userId, CancellationToken ct = default);
    Task<Result<OrderDetailResponse>> GetByOrderNumberAsync(Guid userId, string orderNumber, CancellationToken ct = default);
    Task<Result<StartPaymentResponse>> StartPaymentAsync(Guid userId, string orderNumber, CancellationToken ct = default);
    Task<Result<OrderDetailResponse>> VerifyPaymentAsync(Guid userId, string orderNumber, string token, CancellationToken ct = default);
    Task<Result> CancelAsync(Guid userId, string orderNumber, CancellationToken ct = default);
    Task<PagedResult<OrderSummaryResponse>> GetPagedForAdminAsync(AdminOrderQuery query, CancellationToken ct = default);
    Task<Result<OrderDetailResponse>> GetByOrderNumberForAdminAsync(string orderNumber, CancellationToken ct = default);
}
