namespace LootHub.Application.DTOs.Order;

public sealed record OrderItemRequest(
    Guid ProductId,
    int Quantity
    );
