using LootHub.Application.Common;
using LootHub.Application.DTOs.Category;

namespace LootHub.Application.Contracts.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken ct = default);
    Task<Result<CategoryResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
