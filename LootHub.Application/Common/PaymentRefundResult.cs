namespace LootHub.Application.Common;

public sealed record PaymentRefundResult(
    bool IsSuccess,
    string? FailureReason
    );
