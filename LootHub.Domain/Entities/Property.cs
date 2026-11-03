using LootHub.Domain.Common;

namespace LootHub.Domain.Entities;

public sealed class Property : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<ProductProperty> ProductProperties { get; set; } = [];
}
