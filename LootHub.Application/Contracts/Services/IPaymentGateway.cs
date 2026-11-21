using LootHub.Application.Common;
using LootHub.Domain.Entities;

namespace LootHub.Application.Contracts.Services;

public interface IPaymentGateway
{
    Task<PaymentInitResult> InitializeAsync(Order order, CancellationToken ct = default);
    Task<PaymentVerifyResult> VerifyAsync(string token, CancellationToken ct = default);
    Task<PaymentRefundResult> RefundAsync(Payment payment, CancellationToken ct = default);
}
