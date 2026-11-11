using LootHub.Application.Common;
using LootHub.Application.DTOs.Brand;

namespace LootHub.Application.Contracts.Services;

public interface IBrandService
{
    Task<IEnumerable<BrandResponse>> GetAllAsync(CancellationToken ct = default);
    Task<Result<BrandResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<BrandResponse>> CreateAsync(CreateBrandRequest request, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateBrandRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
