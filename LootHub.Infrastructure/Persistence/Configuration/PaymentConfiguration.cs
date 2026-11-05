using LootHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LootHub.Infrastructure.Persistence.Configuration;

public sealed class PaymentConfiguration : BaseEntityConfiguration<Payment>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.ProviderToken)
            .HasMaxLength(100)
            .IsRequired(false);
        builder.HasIndex(x => x.ProviderToken)
            .IsUnique();
        builder.Property(x => x.ProviderPaymentId)
            .HasMaxLength(100)
            .IsRequired(false);
        builder.HasIndex(x => x.ProviderPaymentId)
            .IsUnique();
        builder.Property(x => x.FailureReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.OwnsOne(x => x.Amount, Amount =>
        {
            Amount.Property(m => m.Amount)
               .HasColumnName("Amount")
               .HasPrecision(18, 2)
               .IsRequired();
            Amount.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });
        builder.Property(x => x.Method)
            .HasMaxLength(20)
            .HasConversion<string>()
            .IsRequired(false);
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(x => x.PaidAtUtc)
            .IsRequired(false);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
