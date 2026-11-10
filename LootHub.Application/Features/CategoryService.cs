using FluentValidation;
using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Application.DTOs.Category;
using LootHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace LootHub.Application.Features;

public class CategoryService(IUnitOfWork uow,
    IValidator<CreateCategoryRequest> createValidator,
    IValidator<UpdateCategoryRequest> updateValidator
    ) : ICategoryService
{
    public async Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken ct)
    {
        var validationResult = await createValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result<CategoryResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var slug = request.Name.ToKebabCase();
        if (await uow.Categories.AnyAsync(c => c.Slug == slug, cancellationToken: ct))
            return Result<CategoryResponse>.Failure(ErrorType.Conflict, "A category with this name already exists.");

        var category = new Category
        {
            Name = request.Name.Trim(),
            Slug = slug
        };
        await uow.Categories.AddAsync(category, ct);
        await uow.SaveChangesAsync(ct);

        return Result<CategoryResponse>.Success(ToResponse(category));
    }
    public async Task<Result> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken ct)
    {
        var validationResult = await updateValidator.ValidateAsync(request, ct);
        if (!validationResult.IsValid)
            return Result.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var slug = request.Name.ToKebabCase();
        var category = await uow.Categories.GetAsync(new QueryOptions<Category>
        {
            Predicate = c => c.Id == id,
            IsTracking = true
        }, ct);
        if (category is null)
            return Result.Failure(ErrorType.NotFound, "Category not found.");

        if (await uow.Categories.AnyAsync(c => c.Slug == slug && c.Id != id, cancellationToken: ct))
            return Result.Failure(ErrorType.Conflict, "A category with this name already exists.");

        category.Name = request.Name.Trim();
        category.Slug = slug;

        uow.Categories.Update(category);
        await uow.SaveChangesAsync(ct);

        return Result.Success();

    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct)
    {
        var category = await uow.Categories.GetAsync(new QueryOptions<Category>
        {
            Predicate = c => c.Id == id,
            IsTracking = true
        }, ct);
        if (category is null)
            return Result.Failure(ErrorType.NotFound, "Category not found.");
        if (await uow.Products.AnyAsync(p => p.CategoryId == id, cancellationToken: ct))
            return Result.Failure(ErrorType.Conflict, "This category has products and cannot be deleted.");

        uow.Categories.Delete(category);
        await uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(CancellationToken ct)
    {
        var categories = await uow.Categories.GetListAsync(new QueryOptions<Category> { OrderBy = q => q.OrderBy(c => c.Name)}, ct);

        return categories.Select(ToResponse).ToList();
    }

    public async Task<Result<CategoryResponse>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var category = await uow.Categories.GetAsync(new QueryOptions<Category> { Predicate = c => c.Id == id }, ct);
        if(category is null)
            return Result<CategoryResponse>.Failure(ErrorType.NotFound, "Category not found.");

        return Result<CategoryResponse>.Success(ToResponse(category));
    }

    private static CategoryResponse ToResponse(Category category) => new(category.Id, category.Name, category.Slug);
}
