using LootHub.Application.Contracts.Repositories;
using LootHub.Domain.Entities;

namespace LootHub.Application.Common;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    IRepository<Brand> Brands { get; }
    IRepository<Category> Categories { get; }
    IRepository<Property> Properties { get; }
    IRepository<ProductProperty> ProductProperties { get; }
    IRepository<UserAddress> UserAddresses { get; }
    IRepository<OrderItem> OrderItems { get; }
    IRepository<Payment> Payments { get; }

}
