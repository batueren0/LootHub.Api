using LootHub.Application.Common;
using LootHub.Application.DTOs.Products;

namespace LootHub.Application.Contracts.Services;

public interface IProductService
{
    Task<Result<ProductDetailResponse>> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<Result<ProductDetailResponse>> GetBySlugAsync(string slug, CancellationToken ct = default);
}
