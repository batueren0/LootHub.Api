using LootHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LootHub.Infrastructure.Persistence.Configuration;

public sealed class UserAddressConfiguration : BaseEntityConfiguration<UserAddress>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserAddress> builder)
    {
        builder.Property(x => x.Title)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(x => new {x.UserId, x.Title})
            .IsUnique()
            .HasFilter("[IsActive] = 1");
        builder.Property(x => x.Details)
            .HasMaxLength(128)
            .IsRequired();
        builder.Property(x => x.City)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.Country)
            .HasMaxLength (50)
            .IsRequired();


    }
}
