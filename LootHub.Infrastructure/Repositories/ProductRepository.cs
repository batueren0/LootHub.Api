using LootHub.Application.Common;
using LootHub.Application.Contracts.Repositories;
using LootHub.Application.DTOs.Products;
using LootHub.Domain.Entities;
using LootHub.Infrastructure.Common;
using LootHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LootHub.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : BaseRepository<Product, AppDbContext>(context), IProductRepository
{
    public async Task<PagedResult<Product>> GetPagedAsync(ProductQuery query, CancellationToken ct = default)
    {
        var q = Table.AsNoTracking();
        if(!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(p => p.Name.Contains(query.Search));
        if (query.CategoryId is not null)
            q = q.Where(p => p.CategoryId == query.CategoryId);
        if (query.BrandId is not null)
            q = q.Where(p => p.BrandId == query.BrandId);
        if(query.MinPrice is not null)
            q = q.Where(p => p.UnitPrice.Amount >= query.MinPrice);
        if(query.MaxPrice is not null)
            q = q.Where(p => p.UnitPrice.Amount <= query.MaxPrice);
        if(query.InStockOnly)
            q = q.Where(p => p.Stock > 0);

        var totalCount = await q.CountAsync(ct);

        q = query.Sort switch
        {
            ProductSort.PriceAsc => q.OrderBy(p => p.UnitPrice.Amount).ThenBy(p => p.Id),
            ProductSort.PriceDesc => q.OrderByDescending(p => p.UnitPrice.Amount).ThenBy(p => p.Id),
            ProductSort.NameAsc => q.OrderBy(p => p.Name).ThenBy(p => p.Id),
            _ => q.OrderByDescending(p => p.CreatedAtUtc).ThenBy(p => p.Id)
        };

        var items = await q
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .ToListAsync(ct);

        return new PagedResult<Product>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount,
        };
    }
}
