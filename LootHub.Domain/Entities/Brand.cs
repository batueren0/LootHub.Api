using LootHub.Domain.Common;

namespace LootHub.Domain.Entities;

public sealed class Brand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = [];
}
