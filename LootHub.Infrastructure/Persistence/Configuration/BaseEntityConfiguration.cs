using LootHub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LootHub.Infrastructure.Persistence.Configuration;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IsActive)
            .IsRequired();
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
        builder.Property(x => x.LastModifiedAtUtc)
            .IsRequired();
        builder.Property(x => x.DeletedAtUtc)
            .IsRequired(false);
        builder.HasIndex(x => x.IsActive)
            .HasFilter("[IsActive] = 1");
        builder.HasIndex(x => x.CreatedAtUtc);
        builder.HasQueryFilter(x => x.IsActive);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        ConfigureEntity(builder);
    }

    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
