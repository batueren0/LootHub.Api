using LootHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LootHub.Infrastructure.Persistence.Configuration;

public sealed class OrderConfiguration : BaseEntityConfiguration<Order>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(x => x.Title)
                .HasMaxLength(50)
                .IsRequired()
                .HasColumnName("AddressTitle");
            address.Property(x => x.Details)
                .HasMaxLength(128)
                .IsRequired()
                .HasColumnName("AddressDetails");
            address.Property(x => x.City)
                .HasMaxLength(50)
                .IsRequired()
                .HasColumnName("AddressCity");
            address.Property(x => x.Country)
                .HasMaxLength(50)
                .IsRequired()
                .HasColumnName("AddressCountry");
        });

        builder.Property(x => x.OrderNumber)
            .HasMaxLength(30)
            .IsRequired();
        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();
        builder.Property(x => x.CustomerName)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(x => x.CustomerEmail)
            .HasMaxLength(70)
            .IsRequired();
        builder.Property(x => x.Phone)
            .HasMaxLength(20)
            .IsRequired();
        builder.OwnsOne(x => x.TotalAmount, TotalAmount =>
        {
            TotalAmount.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2)
                .IsRequired();
            TotalAmount.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        builder.Property(x => x.Status)
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
