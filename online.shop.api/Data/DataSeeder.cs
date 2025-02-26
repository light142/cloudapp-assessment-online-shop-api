using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using online.shop.api.Models;

namespace online.shop.api.Data
{
    public static class DataSeeder
    {
        public static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            if (!await context.Products.AnyAsync())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Apple iPhone 15",
                        Description = "Latest Apple smartphone",
                        Price = 999.99m,
                        ImageUrl = "https://placehold.co/1000"
                    },
                    new Product
                    {
                        Name = "Samsung Galaxy S24",
                        Description = "Latest Samsung smartphone",
                        Price = 899.99m,
                        ImageUrl = "https://placehold.co/1000"
                    },
                    new Product
                    {
                        Name = "Sony PlayStation 5",
                        Description = "Next-gen gaming console",
                        Price = 499.99m,
                        ImageUrl = "https://placehold.co/1000"
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await EnsureRoleExists(roleManager, "Admin");
            await EnsureRoleExists(roleManager, "User");

            await EnsureUserExists(userManager, "admin@ess.ais.ac.nz", "Admin", "P@ssw0rd$");
            await EnsureUserExists(userManager, "user@ess.ais.ac.nz", "User", "P@ssw0rd$");
        }

        private static async Task EnsureRoleExists(
            RoleManager<IdentityRole> roleManager,
            string roleName
        )
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static async Task EnsureUserExists(
            UserManager<ApplicationUser> userManager,
            string email,
            string role,
            string password
        )
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
