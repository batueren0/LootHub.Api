using LootHub.Domain.Enums;

namespace LootHub.Application.DTOs.Order;

public sealed record AdminOrderQuery
{
    public OrderStatus? Status { get; init; }
    public string? Search { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
