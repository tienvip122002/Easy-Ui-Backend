using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace EasyUiBackend.Infrastructure.Seeds
{
    public static class DefaultData
    {
        public static async Task SeedDataAsync(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
            await SeedUIComponentsAsync(context);
            await SeedCategoriesAsync(context);
            await SeedTagsAsync(context);
            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Creator"))
            {
                await roleManager.CreateAsync(new ApplicationRole("Creator"));
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@example.com") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Admin123!");
                await userManager.AddToRoleAsync(user, "Admin");
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
                        Name = "Primary Button",
                        Description = "A standard button with primary style",
                        Html = "<button class=\"btn-primary\">Click me</button>",
                        Css = ".btn-primary { background-color: blue; color: white; }",
                        Js = "document.querySelector('.btn-primary').addEventListener('click', function() { alert('Button clicked!'); });",
                        PreviewUrl = "https://example.com/button.png",
                        Type = "component",
                        Framework = "HTML/CSS/JS"
                    }
                };
                await context.UIComponents.AddRangeAsync(components);
            }
        }

        private static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Buttons", Description = "All types of buttons" },
                    new Category { Name = "Forms", Description = "Form components and layouts" },
                    new Category { Name = "Navigation", Description = "Navigation components" }
                };
                await context.Categories.AddRangeAsync(categories);
            }
        }

        private static async Task SeedTagsAsync(AppDbContext context)
        {
            if (!context.Tags.Any())
            {
                var tags = new List<Tag>
                {
                    new Tag { Name = "responsive" },
                    new Tag { Name = "tailwind" },
                    new Tag { Name = "dark-mode" }
                };
                await context.Tags.AddRangeAsync(tags);
            }
        }
    }
} 