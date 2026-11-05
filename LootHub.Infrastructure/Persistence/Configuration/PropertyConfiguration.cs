using LootHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LootHub.Infrastructure.Persistence.Configuration;

public sealed class PropertyConfiguration : BaseEntityConfiguration<Property>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Property> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("[IsActive] = 1");

    }
}
