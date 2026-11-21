using LootHub.Application.Common;
using LootHub.Application.Contracts.Services;
using LootHub.Domain.Entities;
using LootHub.Infrastructure.Authentication;
using LootHub.Infrastructure.Common;
using LootHub.Infrastructure.Payments;
using LootHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LootHub.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
       .AddRoles<AppRole>()
       .AddEntityFrameworkStores<AppDbContext>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<IyzicoOptions>(configuration.GetSection(IyzicoOptions.SectionName));


        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        if (configuration["Payments:Provider"] == "Iyzico")
        {
            if (string.IsNullOrWhiteSpace(configuration["Iyzico:ApiKey"]) || string.IsNullOrWhiteSpace(configuration["Iyzico:SecretKey"]))
                throw new InvalidOperationException("Iyzico keys are missing. Add them to User Secrets.");

            services.AddScoped<IPaymentGateway, IyzicoPaymentGateway>();
        }
            
        else
            services.AddScoped<IPaymentGateway, FakePaymentGateway>();

        return services;
    }
}
