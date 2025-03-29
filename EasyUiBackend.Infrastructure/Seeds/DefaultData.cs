using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace EasyUiBackend.Infrastructure.Seeds
{
    public static class DefaultData
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            // Seed Roles
            await SeedRolesAsync(roleManager);
            
            // Seed Admin User
            await SeedAdminUserAsync(userManager);
            
            // Seed UI Components
            await SeedUIComponentsAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));
        }

        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                FullName = "System Administrator"
            };

            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedUIComponentsAsync(AppDbContext context)
        {
            if (!context.UIComponents.Any())
            {
                var components = new List<UIComponent>
                {
                    new UIComponent 
                    { 
                        Id = Guid.NewGuid(),
                        Name = "Button Component",
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent 
                    { 
                        Id = Guid.NewGuid(),
                        Name = "Input Component",
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent 
                    { 
                        Id = Guid.NewGuid(),
                        Name = "Table Component",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.UIComponents.AddRangeAsync(components);
                await context.SaveChangesAsync();
            }
        }
    }
} 