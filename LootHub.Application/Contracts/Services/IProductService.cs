using LootHub.Application.Common;
using LootHub.Application.DTOs.Products;
using LootHub.Domain.Enums;

namespace LootHub.Application.Contracts.Services;

public interface IProductService
{
    Task<Result<ProductDetailResponse>> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<Result<ProductDetailResponse>> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<PagedResult<ProductSummaryResponse>> GetPagedAsync(ProductQuery query, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> UpdateStockAsync(Guid id ,UpdateStockRequest request, CancellationToken ct = default);
}
