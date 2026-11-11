namespace LootHub.Application.DTOs.Brand;

public sealed record BrandResponse(
    Guid Id,
    string Name,
    string Slug
    );
