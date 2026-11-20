using LootHub.Domain.Enums;

namespace LootHub.Application.DTOs.Order;

public sealed record UpdateOrderStatusRequest(
    OrderStatus Status
    );
