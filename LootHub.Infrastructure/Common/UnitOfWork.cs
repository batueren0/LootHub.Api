using LootHub.Application.Common;
using LootHub.Application.Contracts.Repositories;
using LootHub.Domain.Entities;
using LootHub.Infrastructure.Persistence.Context;
using LootHub.Infrastructure.Repositories;

namespace LootHub.Infrastructure.Common;

public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public IProductRepository Products { get; } = new ProductRepository(context);

    public IOrderRepository Orders { get; } = new OrderRepository(context);
    public IRepository<Brand> Brands { get; } = new BaseRepository<Brand, AppDbContext>(context);
    public IRepository<Category> Categories { get; } = new BaseRepository<Category, AppDbContext>(context);
    public IRepository<Property> Properties { get; } = new BaseRepository<Property, AppDbContext>(context);
    public IRepository<ProductProperty> ProductProperties { get; } = new BaseRepository<ProductProperty, AppDbContext>(context);
    public IRepository<UserAddress> UserAddresses { get; } = new BaseRepository<UserAddress, AppDbContext>(context);
    public IRepository<Payment> Payments { get; } = new BaseRepository<Payment, AppDbContext>(context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => context.SaveChangesAsync(cancellationToken);
}
