using Microsoft.AspNetCore.Identity;

namespace LootHub.Domain.Entities;

public sealed class AppRole : IdentityRole<Guid>
{
    public DateTimeOffset CreatedAtUtc { get; set; }

    public AppRole()
    {
        var time = TimeProvider.System.GetUtcNow();
        Id = Guid.CreateVersion7(time);
        CreatedAtUtc = time;
    }
}
