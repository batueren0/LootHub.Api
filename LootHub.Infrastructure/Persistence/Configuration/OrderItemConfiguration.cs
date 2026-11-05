using LootHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LootHub.Infrastructure.Persistence.Configuration;

public sealed class OrderItemConfiguration : BaseEntityConfiguration<OrderItem>
{
    protected override void ConfigureEntity(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(x => x.Quantity)
            .IsRequired();
        builder.Property(x => x.ProductName)
            .HasMaxLength(200)
            .IsRequired();
        builder.Ignore(x => x.TotalAmount);
        builder.OwnsOne(x => x.UnitPrice, unitPrice =>
        {
            unitPrice.Property(m => m.Amount)
               .HasColumnName("UnitPrice")
               .HasPrecision(18, 2)
               .IsRequired();
            unitPrice.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasOne(x => x.Product)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
