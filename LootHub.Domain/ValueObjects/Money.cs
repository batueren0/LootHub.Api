namespace LootHub.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; init; }
    public string Currency {  get; init; } = string.Empty;

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Zero() => new(0 , "TRY");
    public static Money TRY(decimal amount) => new(amount , "TRY");
    public static Money Create(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        if(string.IsNullOrEmpty(currency) || currency.Length != 3)
            throw new ArgumentException("Currency must have 3 character.", nameof(currency));

        return new Money(amount, currency);
    }

    private static void CheckCurrencyCompatibility(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException($"Cannot operate on different currencies: {left.Currency} and {right.Currency}.");
    }

    public static Money operator +(Money left, Money right)
    {
        CheckCurrencyCompatibility(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        CheckCurrencyCompatibility(left, right);
        if (left.Amount - right.Amount < 0)
            throw new ArgumentException("Result cannot be negative", nameof(left));
        return new Money(left.Amount - right.Amount, left.Currency);
    }


    public override string ToString() => $"{Amount:N2} {Currency}";
}
