using LootHub.Domain.Common;
using LootHub.Domain.ValueObjects;

namespace LootHub.Domain.Entities;

public sealed class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Money UnitPrice { get; set; } = Money.Zero();
    public Money TotalAmount => Money.TRY(UnitPrice.Amount * Quantity);
    public Order? Order { get; set; }
    public Product? Product { get; set; }
   
    
}
