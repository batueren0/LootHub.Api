using LootHub.Domain.Common;
using LootHub.Domain.Enums;
using LootHub.Domain.ValueObjects;

namespace LootHub.Domain.Entities;

public sealed class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Money TotalAmount { get; set; } = Money.Zero();
    public Address Address { get; init; } = Address.Empty();
    public OrderStatus Status { get; set; }
    public AppUser? User { get; set; }
    public ICollection<Payment> Payments { get; set; } = [];
    public ICollection<OrderItem> Items { get; set; } = [];

}
