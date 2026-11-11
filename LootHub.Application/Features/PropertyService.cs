using FluentValidation;
using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Property;
using LootHub.Domain.Entities;

namespace LootHub.Application.Features;

public class PropertyService(IUnitOfWork uow,
    IValidator<CreatePropertyRequest> createValidator,
    IValidator<UpdatePropertyRequest> updateValidator
    ) : IPropertyService
{
    public async Task<Result<PropertyResponse>> CreateAsync(CreatePropertyRequest request, CancellationToken ct = default)
    {
        var validationResult = await createValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result<PropertyResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var name = request.Name.Trim();
        if (await uow.Properties.AnyAsync(p => p.Name == name, cancellationToken: ct))
            return Result<PropertyResponse>.Failure(ErrorType.Conflict, "A property with this name already exists.");

        var property = new Property
        {
            Name = name
        };
        await uow.Properties.AddAsync(property, ct);
        await uow.SaveChangesAsync(ct);

        return Result<PropertyResponse>.Success(ToResponse(property));
    }

    public async Task<Result> UpdateAsync(Guid id, UpdatePropertyRequest request, CancellationToken ct = default)
    {
        var validationResult = await updateValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var property = await uow.Properties.GetAsync(new QueryOptions<Property>
        {
            Predicate = p => p.Id == id,
            IsTracking = true,
        }, ct);

        if (property is null)
            return Result.Failure(ErrorType.NotFound, "Property not found.");

        var name = request.Name.Trim();
        if (await uow.Properties.AnyAsync(p => p.Name == name && p.Id != id, cancellationToken: ct))
            return Result.Failure(ErrorType.Conflict, "A property with this name already exists.");

        property.Name = name;

        uow.Properties.Update(property);
        await uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var property = await uow.Properties.GetAsync(new QueryOptions<Property>
        {
            Predicate = p => p.Id == id,
            IsTracking = true
        }, ct);
        if (property is null)
            return Result.Failure(ErrorType.NotFound, "Property not found.");
        if (await uow.ProductProperties.AnyAsync(pp => pp.PropertyId == id, cancellationToken: ct))
            return Result.Failure(ErrorType.Conflict, "This property is used by products and cannot be deleted.");

        uow.Properties.Delete(property);
        await uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<IEnumerable<PropertyResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var properties = await uow.Properties.GetListAsync(new QueryOptions<Property>
        {
            OrderBy = o => o.OrderBy(p => p.Name)
        }, ct);

        return properties.Select(ToResponse).ToList();
    }

    public async Task<Result<PropertyResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var property = await uow.Properties.GetAsync(new QueryOptions<Property>
        {
            Predicate = p => p.Id == id
        }, ct);
        if (property is null)
            return Result<PropertyResponse>.Failure(ErrorType.NotFound, "Property not found");

        return Result<PropertyResponse>.Success(ToResponse(property));
    }

    private static PropertyResponse ToResponse(Property property) => new(property.Id, property.Name);
}
