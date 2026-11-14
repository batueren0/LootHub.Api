using LootHub.Application.Common;
using LootHub.Application.DTOs.Products;
using LootHub.Domain.Entities;

namespace LootHub.Application.Contracts.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<PagedResult<Product>> GetPagedAsync(ProductQuery query, CancellationToken ct = default);
}
