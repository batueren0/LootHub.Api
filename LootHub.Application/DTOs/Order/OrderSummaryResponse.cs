using LootHub.Domain.Enums;

namespace LootHub.Application.DTOs.Order;

public sealed record OrderSummaryResponse(
    Guid Id,
    string OrderNumber,
    string CustomerName,
    DateTimeOffset CreatedAtUtc,
    decimal TotalAmount,
    OrderStatus Status,
    int ItemCount
    );
