namespace LootHub.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset LastModifiedAtUtc { get; set; }
    public DateTimeOffset? DeletedAtUtc { get; set; }

    protected BaseEntity()
    {
        var time = TimeProvider.System.GetUtcNow();

        Id = Guid.CreateVersion7(time);
        IsActive = true;
        CreatedAtUtc = time;
        LastModifiedAtUtc = time;
    }
}
