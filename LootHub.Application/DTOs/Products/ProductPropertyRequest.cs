namespace LootHub.Application.DTOs.Products;

public sealed record ProductPropertyRequest(
    Guid PropertyId,
    string Value
    );
