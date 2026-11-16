using FluentValidation;
using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Address;
using LootHub.Domain.Entities;

namespace LootHub.Application.Features;

public class AddressService(IUnitOfWork uow,
    IValidator<CreateAddressRequest> createValidator,
    IValidator<UpdateAddressRequest> updateValidator
    ) : IAddressService
{
    public async Task<Result<AddressResponse>> CreateAsync(Guid userId, CreateAddressRequest request, CancellationToken ct = default)
    {
        request = request with
        {
            Title = request.Title.Trim(),
            Details = request.Details.Trim(),
            City = request.City.Trim(),
            Country = request.Country.Trim(),
        };
        var validationResult = await createValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result<AddressResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        if (await uow.UserAddresses.AnyAsync(ad => ad.UserId == userId && ad.Title == request.Title, cancellationToken: ct))
            return Result<AddressResponse>.Failure(ErrorType.Conflict, "An address with this name already exists.");

        var address = new UserAddress
        {
            UserId = userId,
            Title = request.Title,
            Details = request.Details,
            City = request.City,
            Country = request.Country,
        };

        await uow.UserAddresses.AddAsync(address, ct);
        await uow.SaveChangesAsync(ct);

        return Result<AddressResponse>.Success(ToResponse(address));

    }

    public async Task<Result> UpdateAsync(Guid userId, Guid id, UpdateAddressRequest request, CancellationToken ct = default)
    {
        request = request with
        {
            Title = request.Title.Trim(),
            Details = request.Details.Trim(),
            City = request.City.Trim(),
            Country = request.Country.Trim(),
        };
        var validationResult = await updateValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var address = await uow.UserAddresses.GetAsync(new QueryOptions<UserAddress>
        {
            Predicate = ad => ad.UserId == userId && ad.Id == id,
            IsTracking = true,
        }, ct);

        if (address is null)
            return Result.Failure(ErrorType.NotFound, "Address not found.");

        if (await uow.UserAddresses.AnyAsync(ad => ad.UserId == userId && ad.Title == request.Title && ad.Id != id, cancellationToken: ct))
            return Result.Failure(ErrorType.Conflict, "An address with this name already exists.");

        address.Title = request.Title;
        address.Details = request.Details;
        address.City = request.City;
        address.Country = request.Country;


        uow.UserAddresses.Update(address);
        await uow.SaveChangesAsync(ct);

        return Result.Success();

    }

    public async Task<Result> DeleteAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var address = await uow.UserAddresses.GetAsync(new QueryOptions<UserAddress>
        {
            Predicate = ad => ad.UserId == userId && ad.Id == id,
            IsTracking = true
        }, ct);
        if (address is null)
            return Result.Failure(ErrorType.NotFound, "Address not found.");

        uow.UserAddresses.Delete(address);
        await uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<IEnumerable<AddressResponse>> GetAllAsync(Guid userId, CancellationToken ct = default)
    {
        var addresses = await uow.UserAddresses.GetListAsync(new QueryOptions<UserAddress>
        {
            Predicate = p => p.UserId == userId,
            OrderBy = o => o.OrderBy(ad => ad.Title)
        },ct);

        return addresses.Select(ToResponse).ToList();
    }

    public async Task<Result<AddressResponse>> GetByIdAsync(Guid userId, Guid id, CancellationToken ct = default)
    {
        var address = await uow.UserAddresses.GetAsync(new QueryOptions<UserAddress>
        {
            Predicate = a => a.UserId == userId && a.Id == id,
        }, ct);
        if(address is null)
            return Result<AddressResponse>.Failure(ErrorType.NotFound, "Address not found.");

        return Result<AddressResponse>.Success(ToResponse(address));
    }


    private static AddressResponse ToResponse(UserAddress address) => new(address.Id, address.Title, address.Details, address.City, address.Country);

}
