using LootHub.Domain.Enums;

namespace LootHub.Application.DTOs.Order;

public sealed record OrderDetailResponse(
    Guid Id,
    string OrderNumber,
    DateTimeOffset CreatedAtUtc,
    decimal TotalAmount,
    OrderStatus Status,
    int ItemCount,
    string Address,
    string CustomerName,
    string Phone,
    IReadOnlyList<OrderItemResponse> Items
    );
