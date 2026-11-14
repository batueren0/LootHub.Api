namespace LootHub.Application.DTOs.Products;

public sealed record ProductQuery
{
    public string? Search { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? BrandId { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public bool InStockOnly { get; init; } = false;
    public ProductSort Sort { get; init; } = ProductSort.Newest;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 12;
}