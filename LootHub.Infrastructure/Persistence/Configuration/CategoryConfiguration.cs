using LootHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LootHub.Infrastructure.Persistence.Configuration;

public sealed class CategoryConfiguration : BaseEntityConfiguration<Category>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Category> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.Slug)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(x => x.Slug)
            .IsUnique()
            .HasFilter("[IsActive] = 1");

    }
}
