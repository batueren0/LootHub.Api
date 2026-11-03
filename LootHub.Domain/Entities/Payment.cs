using LootHub.Domain.Common;
using LootHub.Domain.Enums;
using LootHub.Domain.ValueObjects;

namespace LootHub.Domain.Entities;

public sealed class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public Money Amount { get; set; } = Money.Zero();
    public PaymentMethod? Method { get; set; }
    public PaymentStatus Status { get; set; }
    public Order? Order { get; set; }
    public string? ProviderToken { get; set; }
    public string? ProviderPaymentId { get; set; }
    public string? FailureReason { get; set; }
    public DateTimeOffset? PaidAtUtc { get; set; }

}
