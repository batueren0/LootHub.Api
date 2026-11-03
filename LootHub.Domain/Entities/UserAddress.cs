using LootHub.Domain.Common;

namespace LootHub.Domain.Entities;

public sealed class UserAddress : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; init; } = string.Empty;
    public string Details { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Country {  get; init; } = string.Empty;
    public AppUser? User { get; set; }
}
