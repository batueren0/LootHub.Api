using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Domain.Entities;

namespace LootHub.Infrastructure.Payments;

public class IyzicoPaymentGateway : IPaymentGateway
{
    public Task<PaymentInitResult> InitializeAsync(Order order, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<PaymentRefundResult> RefundAsync(Payment payment, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<PaymentVerifyResult> VerifyAsync(string token, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
