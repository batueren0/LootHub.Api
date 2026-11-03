using LootHub.Domain.Common;

namespace LootHub.Domain.Entities;

public sealed class ProductProperty : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid PropertyId { get; set; }
    public string Value { get; set; } = string.Empty;
    public Product? Product { get; set; }
    public Property? Property { get; set; }
}
