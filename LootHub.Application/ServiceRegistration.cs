using FluentValidation;
using LootHub.Application.Contracts.Services;
using LootHub.Application.Features;
using Microsoft.Extensions.DependencyInjection;

namespace LootHub.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ServiceRegistration).Assembly);

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAddressService, AddressService>();

        return services;
    }
}
