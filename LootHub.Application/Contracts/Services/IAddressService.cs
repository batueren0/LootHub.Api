using LootHub.Application.Common;
using LootHub.Application.DTOs.Address;

namespace LootHub.Application.Contracts.Services;

public interface IAddressService
{
    Task<IEnumerable<AddressResponse>> GetAllAsync(Guid userId, CancellationToken ct = default);
    Task<Result<AddressResponse>> GetByIdAsync(Guid userId, Guid id, CancellationToken ct = default);
    Task<Result<AddressResponse>> CreateAsync(Guid userId, CreateAddressRequest request, CancellationToken ct = default);
    Task<Result> UpdateAsync(Guid userId, Guid id, UpdateAddressRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid userId, Guid id, CancellationToken ct = default);
}
