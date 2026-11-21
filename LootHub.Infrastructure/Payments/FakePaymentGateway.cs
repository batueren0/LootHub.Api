using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Domain.Entities;

namespace LootHub.Infrastructure.Payments;

public sealed class FakePaymentGateway : IPaymentGateway
{
    private const decimal FailAboveAmount = 100_000m;
    public Task<PaymentInitResult> InitializeAsync(Order order, CancellationToken ct = default)
    {
        var prefix = order.TotalAmount.Amount > FailAboveAmount ? "FAIL-" : "FAKE-";
        var token = $"{prefix}{Guid.NewGuid():N}";
        var pageUrl = $"https://fake-pay.local/checkout?token={token}";

        return Task.FromResult(new PaymentInitResult(true, token, pageUrl, null));
    }

    public Task<PaymentRefundResult> RefundAsync(Payment payment, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(payment.ProviderPaymentId))
            return Task.FromResult(new PaymentRefundResult(false, "No completed payment to refund."));

        return Task.FromResult(new PaymentRefundResult(true, null));
    }

    public Task<PaymentVerifyResult> VerifyAsync(string token, CancellationToken ct = default)
    {
        if (token.StartsWith("FAIL-"))
            return Task.FromResult(new PaymentVerifyResult(false, null, "Card declined (simulated: amount above limit)"));

        var providerPaymentId = $"FAKEPAY-{Guid.NewGuid():N}";
        return Task.FromResult(new PaymentVerifyResult(true, providerPaymentId, null));

    }
}
