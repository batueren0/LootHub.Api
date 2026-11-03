using LootHub.Domain.Common;
using LootHub.Domain.ValueObjects;

namespace LootHub.Domain.Entities;

public sealed class Product : BaseEntity
{
    public Guid BrandId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Money UnitPrice { get; set; } = Money.Zero();
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public Brand? Brand { get; set; }
    public Category? Category { get; set; }
    public ICollection<ProductProperty> ProductProperties { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];

}
