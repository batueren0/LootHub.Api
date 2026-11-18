namespace LootHub.Application.DTOs.Order;

public sealed record StartPaymentResponse(
    string Token,
    string PaymentUrl
    );
