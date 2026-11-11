using LootHub.Application.Common;
using LootHub.Application.DTOs.Property;

namespace LootHub.Application.Contracts.Services;

public interface IPropertyService
{
    Task<IEnumerable<PropertyResponse>> GetAllAsync(CancellationToken ct = default);
    Task<Result<PropertyResponse>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PropertyResponse>> CreateAsync(CreatePropertyRequest request, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid id, UpdatePropertyRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
