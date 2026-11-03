using Microsoft.AspNetCore.Identity;

namespace LootHub.Domain.Entities;

public sealed class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public ICollection<UserAddress> Addresses { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];

    public AppUser()
    {
        var time = TimeProvider.System.GetUtcNow();
        Id = Guid.CreateVersion7(time);
        SecurityStamp = Guid.NewGuid().ToString();
        CreatedAtUtc = time;
    }
}
