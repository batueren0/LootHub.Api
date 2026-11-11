using FluentValidation;
using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Brand;
using LootHub.Application.DTOs.Category;
using LootHub.Domain.Entities;

namespace LootHub.Application.Features;

public class BrandService(IUnitOfWork uow,
    IValidator<CreateBrandRequest> createValidator,
    IValidator<UpdateBrandRequest> updateValidator
    ) : IBrandService
{
    public async Task<Result<BrandResponse>> CreateAsync(CreateBrandRequest request, CancellationToken ct = default)
    {
        var validationResult = await createValidator.ValidateAsync(request, cancellation: ct);
        if (!validationResult.IsValid)
            return Result<BrandResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var slug = request.Name.ToKebabCase();
        if (await uow.Brands.AnyAsync(b => b.Slug == slug, cancellationToken: ct))
            return Result<BrandResponse>.Failure(ErrorType.Conflict, "A brand with this name already exists.");

        var brand = new Brand
        {
            Name = request.Name.Trim(),
            Slug = slug,
        };
        await uow.Brands.AddAsync(brand, ct);
        await uow.SaveChangesAsync(ct);

        return Result<BrandResponse>.Success(ToResponse(brand));
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateBrandRequest request, CancellationToken ct = default)
    {
        var validationResult = await updateValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var slug = request.Name.ToKebabCase();
        var brand = await uow.Brands.GetAsync(new QueryOptions<Brand>
        {
            Predicate = b => b.Id == id,
            IsTracking = true,
        },ct);
        if (brand is null)
            return Result.Failure(ErrorType.NotFound, "Brand not found.");

        if (await uow.Brands.AnyAsync(b => b.Slug == slug && b.Id != id, cancellationToken: ct))
            return Result.Failure(ErrorType.Conflict, "A brand with this name already exists.");

        brand.Name = request.Name.Trim();
        brand.Slug = slug;

        uow.Brands.Update(brand);
        await uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var brand = await uow.Brands.GetAsync(new QueryOptions<Brand>
        {
            Predicate = b => b.Id == id,
            IsTracking = true
        }, ct);
        if(brand is null)
            return Result.Failure(ErrorType.NotFound, "Brand not found.");
        if (await uow.Products.AnyAsync(p => p.BrandId == id, cancellationToken: ct))
            return Result.Failure(ErrorType.Conflict, "This brand has products and cannot be deleted.");

        uow.Brands.Delete(brand);
        await uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<IEnumerable<BrandResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var brands = await uow.Brands.GetListAsync(new QueryOptions<Brand>
        {
            OrderBy = q => q.OrderBy(b => b.Name)
        }, ct);

        return brands.Select(ToResponse).ToList();
    }

    public async Task<Result<BrandResponse>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var brand = await uow.Brands.GetAsync(new QueryOptions<Brand>
        {
            Predicate = b => b.Id == id,
        }, ct);
        if (brand is null)
            return Result<BrandResponse>.Failure(ErrorType.NotFound, "Brand not found.");

        return Result<BrandResponse>.Success(ToResponse(brand));
    }

    private static BrandResponse ToResponse(Brand brand) => new(brand.Id, brand.Name, brand.Slug);
}
