using LootHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LootHub.Infrastructure.Persistence.Configuration;

public sealed class ProductConfiguration : BaseEntityConfiguration<Product>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Product> builder)
    {
        builder.OwnsOne(x => x.UnitPrice, price =>
        {
            price.Property(m => m.Amount)
                .HasColumnName("UnitPrice")
                .HasPrecision(18, 2)
                .IsRequired();
            price.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        builder.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();
        builder.Property(x => x.Slug)
            .HasMaxLength(128)
            .IsRequired();
        builder.Property(x => x.Description)
            .IsRequired();
        builder.HasIndex(x => x.Slug)
            .HasFilter("[IsActive] = 1")
            .IsUnique();
        builder.Property(x => x.ImageUrl)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.ToTable(s => s.HasCheckConstraint("CK_Products_Stock", "[Stock] >= 0"));



        builder.HasOne(x => x.Brand)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);


    }
}
