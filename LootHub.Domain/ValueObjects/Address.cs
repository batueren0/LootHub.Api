namespace LootHub.Domain.ValueObjects;

public sealed record Address
{
    public string Title { get; init; }
    public string Details { get; init; }
    public string City { get; init; }
    public string Country { get; init; }

    public Address(string title, string details, string city, string country)
    {
        Title = title;
        Details = details;
        City = city;
        Country = country;
    }

    public static Address Empty() => new(string.Empty, string.Empty, string.Empty, string.Empty);
    public static Address Create(string title, string details, string city, string country)
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(details) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(country))
            throw new ArgumentException("All address fields are required.");

        return new Address(title, details, city, country);
    }

    public override string ToString() => $"{Title}, {Details}, {City}/{Country}";
}
