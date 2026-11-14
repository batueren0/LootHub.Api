using FluentValidation;
using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Products;
using LootHub.Domain.Entities;
using LootHub.Domain.Enums;
using LootHub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LootHub.Application.Features;

public class ProductService(IUnitOfWork uow,
    IValidator<CreateProductRequest> createValidator,
    IValidator<UpdateProductRequest> updateValidator,
    IValidator<UpdateStockRequest> stockValidator) : IProductService
{
    public async Task<Result<ProductDetailResponse>> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        var validationResult = await createValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result<ProductDetailResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var slug = request.Name.ToKebabCase();
        if (await uow.Products.AnyAsync(p => p.Slug == slug, cancellationToken: ct))
            return Result<ProductDetailResponse>.Failure(ErrorType.Conflict, "A product with this name already exists.");

        var propertyIds = request.Properties.Select(p => p.PropertyId).ToList();
        var referenceError = await ValidateReferencesAsync(request.BrandId, request.CategoryId, propertyIds, ct);
        if(referenceError is not null)
            return Result<ProductDetailResponse>.Failure(ErrorType.Validation, referenceError);

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

    public async Task<Result> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct)
    {
        var validationResult = await updateValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var product = await uow.Products.GetAsync(new QueryOptions<Product>
        {
            Predicate = p => p.Id == id,
            IsTracking = true,
            Include = p => p.Include(p => p.ProductProperties)
        },ct);
        if (product is null)
            return Result.Failure(ErrorType.NotFound, "Product not found.");

        var slug = request.Name.ToKebabCase();
        if (await uow.Products.AnyAsync(p => p.Slug == slug && p.Id != id, cancellationToken: ct))
            return Result.Failure(ErrorType.Conflict, "A product with this name already exists.");

        var propertyIds = request.Properties.Select(p => p.PropertyId).ToList();
        var referenceError = await ValidateReferencesAsync(request.BrandId, request.CategoryId, propertyIds, ct);
        if (referenceError is not null)
            return Result.Failure(ErrorType.Validation, referenceError);

        product.Name = request.Name.Trim();
        product.Slug = slug;
        product.Description = request.Description.Trim();
        product.UnitPrice = Money.TRY(request.Price);
        product.Stock = request.Stock;
        product.ImageUrl = request.ImageUrl;
        product.BrandId = request.BrandId;
        product.CategoryId = request.CategoryId;

        var requested = request.Properties.ToDictionary(p => p.PropertyId, p => p.Value.Trim());

        foreach(var pp in product.ProductProperties.ToList())
        {
            if (requested.TryGetValue(pp.PropertyId, out var newValue))
                pp.Value = newValue;
            else
                uow.ProductProperties.Delete(pp);
        }

        var existingIds = product.ProductProperties.Select(pp => pp.PropertyId).ToHashSet();
        foreach (var (propertyId, value) in requested.Where(r => !existingIds.Contains(r.Key)))
            product.ProductProperties.Add(new ProductProperty { PropertyId = propertyId, Value = value });

        uow.Products.Update(product);
        await uow.SaveChangesAsync(ct);

        return Result.Success();

    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct)
    {
        var product = await uow.Products.GetAsync(new QueryOptions<Product>
        {
            Predicate = p => p.Id == id,
            IsTracking = true,
        },ct);
        if (product is null)
            return Result.Failure(ErrorType.NotFound, "Product not found.");

        uow.Products.Delete(product);
        await uow.SaveChangesAsync(ct);

        return Result.Success();
    }


    public async Task<Result<ProductDetailResponse>> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var product = await GetDetailAsync(p => p.Slug == slug, ct);

        if(product is null)
            return Result<ProductDetailResponse>.Failure(ErrorType.NotFound, "Product not found.");

        return Result<ProductDetailResponse>.Success(ToDetailResponse(product));
    }

    public async Task<PagedResult<ProductSummaryResponse>> GetPagedAsync(ProductQuery query, CancellationToken ct = default)
    {
        query = query with
        {
            Page = Math.Max(query.Page, 1),
            PageSize = Math.Clamp(query.PageSize, 1, 48)
        };

        var paged = await uow.Products.GetPagedAsync(query, ct);

        return new PagedResult<ProductSummaryResponse>
        {
            Items = paged.Items.Select(ToSummaryResponse).ToList(),
            Page = paged.Page,
            PageSize = paged.PageSize,
            TotalCount = paged.TotalCount
        };
    }

    public async Task<Result> UpdateStockAsync(Guid id, UpdateStockRequest request, CancellationToken ct = default)
    {
        var validationResult = await stockValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var product = await uow.Products.GetAsync(new QueryOptions<Product>
        {
            Predicate = p => p.Id == id,
            IsTracking = true,
        }, ct);
        if (product is null)
            return Result.Failure(ErrorType.NotFound, "Product not found.");


        switch (request.Action)
        {
            case StockAction.Renew:
                product.Stock = request.Quantity;
                break;

            case StockAction.Increase:
                product.Stock += request.Quantity;
                break;

            case StockAction.Decrease:
                if (request.Quantity > product.Stock)
                    return Result.Failure(ErrorType.Conflict, "Not enough stock.");
                product.Stock -= request.Quantity;
                break;
        }

        uow.Products.Update(product);
        await uow.SaveChangesAsync(ct);

        return Result.Success();
    }


    private static ProductSummaryResponse ToSummaryResponse(Product p) => new(
        p.Id, p.Name, p.Slug,
        p.UnitPrice.Amount, p.UnitPrice.Currency,
        p.ImageUrl,
        p.Brand!.Name, p.Category!.Name,
        p.Stock);

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

    
    private async Task<string?> ValidateReferencesAsync(Guid brandId, Guid categoryId, IReadOnlyList<Guid> propertyIds, CancellationToken ct)
    {
        if (!await uow.Brands.AnyAsync(b => b.Id == brandId, cancellationToken: ct))
            return "Brand not found.";

        if (!await uow.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken: ct))
            return "Category not found";

        if(propertyIds.Count > 0)
        {
            var existingProperties = await uow.Properties.GetListAsync(new QueryOptions<Property>
            {
                Predicate = p => propertyIds.Contains(p.Id),
            }, ct);

            if (existingProperties.Count() != propertyIds.Count)
                return "One or more properties were not found.";
        }

        return null;
    }

}
