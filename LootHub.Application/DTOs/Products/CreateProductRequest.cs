namespace LootHub.Application.DTOs.Products;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    string? ImageUrl,
    Guid BrandId,
    Guid CategoryId,
    IReadOnlyList<ProductPropertyRequest> Properties
    );
