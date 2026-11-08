using LootHub.Application.Contracts.Repositories;
using LootHub.Domain.Entities;
using LootHub.Infrastructure.Common;
using LootHub.Infrastructure.Persistence.Context;

namespace LootHub.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : BaseRepository<Product, AppDbContext>(context), IProductRepository
{
}
