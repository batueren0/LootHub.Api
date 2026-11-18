namespace LootHub.Application.Common;

public sealed record PaymentInitResult(
    bool IsSuccess,
    string? Token,
    string? PaymentPageUrl,
    string? ErrorMessage
    );
