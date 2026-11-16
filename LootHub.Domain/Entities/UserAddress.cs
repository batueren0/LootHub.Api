using LootHub.Domain.Common;

namespace LootHub.Domain.Entities;

public sealed class UserAddress : BaseEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country {  get; set; } = string.Empty;
    public AppUser? User { get; set; }
}
