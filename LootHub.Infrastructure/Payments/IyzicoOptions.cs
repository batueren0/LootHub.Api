namespace LootHub.Infrastructure.Payments;

public sealed class IyzicoOptions
{
    public const string SectionName = "Iyzico";
    public string ApiKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = string.Empty;
    public string CallbackUrl { get; init; } = string.Empty;
}
