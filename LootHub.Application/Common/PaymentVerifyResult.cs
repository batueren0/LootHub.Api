namespace LootHub.Application.Common;

public sealed record PaymentVerifyResult(
    bool IsSuccess,
    string? ProviderPaymentId,
    string? FailureReason
    );
