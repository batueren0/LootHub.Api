using System.Globalization;
using Iyzipay.Model;
using Iyzipay.Request;
using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Domain.Entities;
using Microsoft.Extensions.Options;
using IyzipayOptions = Iyzipay.Options;
using PaymentEntity = LootHub.Domain.Entities.Payment;

namespace LootHub.Infrastructure.Payments;

public sealed class IyzicoPaymentGateway(IOptions<IyzicoOptions> settings) : IPaymentGateway
{
    private const string SandboxIdentityNumber = "11111111111";
    private const string DefaultCategory = "Gaming Gear";

    private readonly IyzicoOptions _settings = settings.Value;

    private IyzipayOptions ApiOptions => new()
    {
        ApiKey = _settings.ApiKey,
        SecretKey = _settings.SecretKey,
        BaseUrl = _settings.BaseUrl
    };

    public async Task<PaymentInitResult> InitializeAsync(Order order, CancellationToken ct = default)
    {
        var (name, surname) = SplitName(order.CustomerName);

        var address = new Address
        {
            ContactName = order.CustomerName,
            City = order.Address.City,
            Country = order.Address.Country,
            Description = order.Address.Details
        };

        var request = new CreateCheckoutFormInitializeRequest
        {
            Locale = Locale.TR.ToString(),
            ConversationId = order.OrderNumber,
            BasketId = order.OrderNumber,
            Price = FormatPrice(order.TotalAmount.Amount),
            PaidPrice = FormatPrice(order.TotalAmount.Amount),
            Currency = Currency.TRY.ToString(),
            PaymentGroup = PaymentGroup.PRODUCT.ToString(),
            CallbackUrl = _settings.CallbackUrl,
            Buyer = new Buyer
            {
                Id = order.UserId.ToString(),
                Name = name,
                Surname = surname,
                Email = order.CustomerEmail,
                GsmNumber = "+90" + order.Phone,
                IdentityNumber = SandboxIdentityNumber,
                RegistrationAddress = order.Address.Details,
                City = order.Address.City,
                Country = order.Address.Country
            },
            ShippingAddress = address,
            BillingAddress = address,
            BasketItems = order.Items.Select(i => new BasketItem
            {
                Id = i.ProductId.ToString(),
                Name = i.ProductName,
                Category1 = DefaultCategory,
                ItemType = BasketItemType.PHYSICAL.ToString(),
                Price = FormatPrice(i.TotalAmount.Amount)
            }).ToList()
        };

        var response = await CheckoutFormInitialize.Create(request, ApiOptions);

        if (response.Status != "success")
            return new PaymentInitResult(false, null, null, response.ErrorMessage);

        return new PaymentInitResult(true, response.Token, response.PaymentPageUrl, null);
    }

    public async Task<PaymentVerifyResult> VerifyAsync(string token, CancellationToken ct = default)
    {
        var request = new RetrieveCheckoutFormRequest
        {
            Locale = Locale.TR.ToString(),
            Token = token
        };

        var response = await CheckoutForm.Retrieve(request, ApiOptions);
        if (response.Status == "success" && response.PaymentStatus == "SUCCESS")
            return new PaymentVerifyResult(true, response.PaymentId, null);

        return new PaymentVerifyResult(false, null, response.ErrorMessage ?? "Payment failed.");
    }


    public async Task<PaymentRefundResult> RefundAsync(PaymentEntity payment, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(payment.ProviderPaymentId))
            return new PaymentRefundResult(false, "No completed payment to refund.");

        var request = new CreateAmountBasedRefundRequest
        {
            Locale = Locale.TR.ToString(),
            ConversationId = payment.Id.ToString(),
            PaymentId = payment.ProviderPaymentId,
            Price = FormatPrice(payment.Amount.Amount)
        };

        var response = await Refund.CreateAmountBasedRefundRequest(request, ApiOptions);
        if (response.Status == "success")
            return new PaymentRefundResult(true, null);

        return new PaymentRefundResult(false, response.ErrorMessage);
    }

    private static string FormatPrice(decimal amount) =>
        amount.ToString("0.00", CultureInfo.InvariantCulture);

    private static (string Name, string Surname) SplitName(string fullName)
    {
        var trimmed = fullName.Trim();
        var lastSpace = trimmed.LastIndexOf(' ');
        return lastSpace < 0
            ? (trimmed, trimmed)
            : (trimmed[..lastSpace], trimmed[(lastSpace + 1)..]);
    }
}