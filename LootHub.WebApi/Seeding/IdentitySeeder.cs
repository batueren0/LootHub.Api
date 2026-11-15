
using LootHub.Application.Common;
using LootHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LootHub.WebApi.Seeding;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();


        var adminEmail = configuration["Seed:AdminEmail"]
            ?? throw new InvalidOperationException("Seed:AdminEmail is missing.");
        var adminPassword = configuration["Seed:AdminPassword"]
            ?? throw new InvalidOperationException("Seed:AdminPassword is missing. Add it to User Secrets.");

        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            var roleResult = await roleManager.CreateAsync(new AppRole { Name = Roles.Admin });
            if (!roleResult.Succeeded)
                throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                FirstName = "Admin",
                LastName = "Loothub",
                UserName = adminEmail,
                Email = adminEmail,
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
                throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(e => e.Description)));
        }

        if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
        {
            var roleResult = await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            if (!roleResult.Succeeded)
                throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
        }
    }
}
