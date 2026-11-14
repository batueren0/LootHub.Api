using FluentValidation;
using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Category;
using LootHub.Application.DTOs.Products;
using LootHub.Domain.Entities;
using LootHub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LootHub.Application.Features;

public class ProductService(IUnitOfWork uow,
    IValidator<CreateProductRequest> createValidator) : IProductService
{
    public async Task<Result<ProductDetailResponse>> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var validationResult = await createValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result<ProductDetailResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var slug = request.Name.ToKebabCase();
        if (await uow.Products.AnyAsync(p => p.Slug == slug, cancellationToken: ct))
            return Result<ProductDetailResponse>.Failure(ErrorType.Conflict, "A product with this name already exists.");

        if(!await uow.Brands.AnyAsync(b => b.Id == request.BrandId, cancellationToken: ct))
            return Result<ProductDetailResponse>.Failure(ErrorType.Validation, "Product must have a brand.");

        if (!await uow.Categories.AnyAsync(b => b.Id == request.CategoryId, cancellationToken: ct))
            return Result<ProductDetailResponse>.Failure(ErrorType.Validation, "Product must have a category.");

        var propertyIds = request.Properties.Select(p => p.PropertyId).ToList();
        if(propertyIds.Count > 0)
        {
            var existingProperties = await uow.Properties.GetListAsync(new QueryOptions<Property>
            {
                Predicate = p => propertyIds.Contains(p.Id),
            }, ct);

            if(existingProperties.Count() != propertyIds.Count)
                return Result<ProductDetailResponse>.Failure(ErrorType.Validation, "One or more properties were not found.");
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            Slug = slug,
            Description = request.Description.Trim(),
            UnitPrice = Money.TRY(request.Price),
            Stock = request.Stock,
            ImageUrl = request.ImageUrl,
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            ProductProperties = request.Properties.Select(p => new ProductProperty
            {
                PropertyId = p.PropertyId,
                Value = p.Value.Trim()
            }).ToList()
        };

        await uow.Products.AddAsync(product, ct);
        await uow.SaveChangesAsync(ct);

        var created = await GetDetailAsync(p => p.Id == product.Id, ct);
        return Result<ProductDetailResponse>.Success(ToDetailResponse(created!));

    }

    public async Task<Result<ProductDetailResponse>> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var product = await GetDetailAsync(p => p.Slug == slug, ct);

        if(product is null)
            return Result<ProductDetailResponse>.Failure(ErrorType.NotFound, "Product not found.");

        return Result<ProductDetailResponse>.Success(ToDetailResponse(product));
    }



    private Task<Product?> GetDetailAsync(Expression<Func<Product, bool>> predicate, CancellationToken ct) => uow.Products.GetAsync(new QueryOptions<Product>
    {
        Predicate = predicate,
        Include = q => q.Include(p => p.Brand)
                        .Include(p => p.Category)
                        .Include(p => p.ProductProperties).ThenInclude(pp => pp.Property)
    }, ct);

    private static ProductDetailResponse ToDetailResponse(Product p) => new(
        p.Id, p.BrandId, p.CategoryId,
        p.Name, p.Description, p.Slug,
        p.UnitPrice.Amount, p.UnitPrice.Currency,
        p.ImageUrl,
        p.Brand!.Name, p.Category!.Name,
        p.Stock,
        p.ProductProperties.Select(pp => new ProductPropertyResponse(pp.Property!.Name, pp.Value)).ToList());
}
