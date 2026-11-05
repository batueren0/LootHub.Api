using LootHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LootHub.Infrastructure.Persistence.Configuration;

public sealed class ProductPropertyConfiguration : BaseEntityConfiguration<ProductProperty>
{
    protected override void ConfigureEntity(EntityTypeBuilder<ProductProperty> builder)
    {
        builder.Property(x => x.Value)
            .HasMaxLength(150)
            .IsRequired();
        builder.HasIndex(x => new { x.ProductId, x.PropertyId })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.HasOne(x => x.Product)
            .WithMany(x => x.ProductProperties)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Property)
            .WithMany(x => x.ProductProperties)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
