using InventoryInvoiceApp.Models;
using Microsoft.AspNetCore.Identity;

namespace InventoryInvoiceApp.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            IServiceProvider services)
        {
            var roleManager =
                services.GetRequiredService<
                    RoleManager<IdentityRole>>();

            var userManager =
                services.GetRequiredService<
                    UserManager<AppUser>>();

            var configuration =
                services.GetRequiredService<
                    IConfiguration>();

            string[] roles =
            {
                "Admin",
                "Warehouse",
                "Cashier"
            };

            foreach (string roleName in roles)
            {
                bool exists =
                    await roleManager.RoleExistsAsync(
                        roleName);

                if (!exists)
                {
                    var roleResult =
                        await roleManager.CreateAsync(
                            new IdentityRole(roleName));

                    if (!roleResult.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Could not create role: {roleName}");
                    }
                }
            }

            string? adminEmail =
                configuration["SeedAdmin:Email"];

            string? adminPassword =
                configuration["SeedAdmin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "Admin credentials are missing from User Secrets.");
            }

            var admin =
                await userManager.FindByEmailAsync(
                    adminEmail);

            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult =
                    await userManager.CreateAsync(
                        admin,
                        adminPassword);

                if (!createResult.Succeeded)
                {
                    string errors = string.Join(
                        ", ",
                        createResult.Errors.Select(
                            x => x.Description));

                    throw new InvalidOperationException(
                        $"Could not create Admin: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(
                admin,
                "Admin"))
            {
                await userManager.AddToRoleAsync(
                    admin,
                    "Admin");
            }
        }
    }
}