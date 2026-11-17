namespace LootHub.Application.DTOs.Order;

public sealed record CreateOrderRequest(
    Guid AddressId,
    IReadOnlyList<OrderItemRequest> Items
    );
