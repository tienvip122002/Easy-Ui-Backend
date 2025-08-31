using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using System;

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
                        Description = "A standard button with primary style and hover effects",
                        Html = "<button class=\"btn-primary\">Click me</button>",
                        Css = ".btn-primary { background-color: #007bff; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; transition: all 0.3s ease; } .btn-primary:hover { background-color: #0056b3; transform: translateY(-2px); box-shadow: 0 4px 8px rgba(0,0,0,0.2); }",
                        Js = "document.querySelector('.btn-primary').addEventListener('click', function() { console.log('Button clicked!'); });",
                        PreviewUrl = "https://example.com/button.png",
                        Type = "component",
                        Framework = "HTML/CSS/JS",
                        Price = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent
                    {
                        Name = "Card Component",
                        Description = "A modern card component with shadow and rounded corners",
                        Html = "<div class=\"card\"><div class=\"card-header\">Card Title</div><div class=\"card-body\">Card content goes here</div></div>",
                        Css = ".card { background: white; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); overflow: hidden; margin: 20px; } .card-header { background: #f8f9fa; padding: 15px; border-bottom: 1px solid #dee2e6; font-weight: bold; } .card-body { padding: 20px; }",
                        Js = "",
                        PreviewUrl = "https://example.com/card.png",
                        Type = "component",
                        Framework = "HTML/CSS",
                        Price = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent
                    {
                        Name = "Navigation Bar",
                        Description = "Responsive navigation bar with mobile menu",
                        Html = "<nav class=\"navbar\"><div class=\"nav-brand\">Logo</div><ul class=\"nav-menu\"><li><a href=\"#\">Home</a></li><li><a href=\"#\">About</a></li><li><a href=\"#\">Contact</a></li></ul></nav>",
                        Css = ".navbar { display: flex; justify-content: space-between; align-items: center; padding: 1rem 2rem; background: #333; color: white; } .nav-menu { display: flex; list-style: none; gap: 2rem; } .nav-menu a { color: white; text-decoration: none; }",
                        Js = "// Mobile menu toggle logic here",
                        PreviewUrl = "https://example.com/navbar.png",
                        Type = "component",
                        Framework = "HTML/CSS/JS",
                        Price = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent
                    {
                        Name = "Form Input",
                        Description = "Styled form input with focus effects",
                        Html = "<div class=\"form-group\"><label>Email</label><input type=\"email\" class=\"form-input\" placeholder=\"Enter your email\"></div>",
                        Css = ".form-group { margin-bottom: 1rem; } .form-input { width: 100%; padding: 10px; border: 2px solid #ddd; border-radius: 5px; transition: border-color 0.3s ease; } .form-input:focus { outline: none; border-color: #007bff; box-shadow: 0 0 0 3px rgba(0,123,255,0.1); }",
                        Js = "",
                        PreviewUrl = "https://example.com/form-input.png",
                        Type = "component",
                        Framework = "HTML/CSS",
                        Price = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent
                    {
                        Name = "Modal Dialog",
                        Description = "Popup modal with backdrop and close button",
                        Html = "<div class=\"modal\" id=\"myModal\"><div class=\"modal-content\"><span class=\"close\">&times;</span><h2>Modal Title</h2><p>Modal content here</p></div></div>",
                        Css = ".modal { display: none; position: fixed; z-index: 1000; left: 0; top: 0; width: 100%; height: 100%; background-color: rgba(0,0,0,0.5); } .modal-content { background-color: white; margin: 15% auto; padding: 20px; border-radius: 5px; width: 80%; max-width: 500px; } .close { float: right; font-size: 28px; cursor: pointer; }",
                        Js = "document.querySelector('.close').onclick = function() { document.getElementById('myModal').style.display = 'none'; }",
                        PreviewUrl = "https://example.com/modal.png",
                        Type = "component",
                        Framework = "HTML/CSS/JS",
                        Price = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent
                    {
                        Name = "Alert Component",
                        Description = "Bootstrap-style alert with different variants",
                        Html = "<div class=\"alert alert-success\">This is a success alert!</div>",
                        Css = ".alert { padding: 12px 20px; margin: 10px 0; border-radius: 4px; border-left: 4px solid; } .alert-success { background-color: #d4edda; border-color: #28a745; color: #155724; } .alert-warning { background-color: #fff3cd; border-color: #ffc107; color: #856404; } .alert-danger { background-color: #f8d7da; border-color: #dc3545; color: #721c24; }",
                        Js = "",
                        PreviewUrl = "https://example.com/alert.png",
                        Type = "component",
                        Framework = "HTML/CSS",
                        Price = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent
                    {
                        Name = "Progress Bar",
                        Description = "Animated progress bar with percentage",
                        Html = "<div class=\"progress\"><div class=\"progress-bar\" style=\"width: 75%\">75%</div></div>",
                        Css = ".progress { width: 100%; background-color: #e9ecef; border-radius: 10px; overflow: hidden; } .progress-bar { background-color: #007bff; color: white; text-align: center; padding: 10px; transition: width 0.6s ease; }",
                        Js = "",
                        PreviewUrl = "https://example.com/progress.png",
                        Type = "component",
                        Framework = "HTML/CSS",
                        Price = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent
                    {
                        Name = "Dropdown Menu",
                        Description = "Clickable dropdown with smooth animation",
                        Html = "<div class=\"dropdown\"><button class=\"dropdown-toggle\">Dropdown ▼</button><div class=\"dropdown-menu\"><a href=\"#\">Item 1</a><a href=\"#\">Item 2</a><a href=\"#\">Item 3</a></div></div>",
                        Css = ".dropdown { position: relative; display: inline-block; } .dropdown-toggle { background: #007bff; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; } .dropdown-menu { display: none; position: absolute; background: white; min-width: 160px; box-shadow: 0 8px 16px rgba(0,0,0,0.2); border-radius: 5px; z-index: 1; } .dropdown-menu a { color: black; padding: 12px 16px; text-decoration: none; display: block; } .dropdown-menu a:hover { background-color: #f1f1f1; }",
                        Js = "document.querySelector('.dropdown-toggle').addEventListener('click', function() { const menu = document.querySelector('.dropdown-menu'); menu.style.display = menu.style.display === 'block' ? 'none' : 'block'; });",
                        PreviewUrl = "https://example.com/dropdown.png",
                        Type = "component",
                        Framework = "HTML/CSS/JS",
                        Price = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent
                    {
                        Name = "Image Gallery",
                        Description = "Responsive image grid with hover effects",
                        Html = "<div class=\"gallery\"><div class=\"gallery-item\"><img src=\"image1.jpg\" alt=\"Image 1\"></div><div class=\"gallery-item\"><img src=\"image2.jpg\" alt=\"Image 2\"></div><div class=\"gallery-item\"><img src=\"image3.jpg\" alt=\"Image 3\"></div></div>",
                        Css = ".gallery { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 20px; padding: 20px; } .gallery-item { overflow: hidden; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); transition: transform 0.3s ease; } .gallery-item:hover { transform: scale(1.05); } .gallery-item img { width: 100%; height: 200px; object-fit: cover; }",
                        Js = "",
                        PreviewUrl = "https://example.com/gallery.png",
                        Type = "component",
                        Framework = "HTML/CSS",
                        Price = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new UIComponent
                    {
                        Name = "Sidebar Navigation",
                        Description = "Collapsible sidebar with navigation links",
                        Html = "<div class=\"sidebar\"><div class=\"sidebar-header\">Menu</div><nav class=\"sidebar-nav\"><a href=\"#\" class=\"nav-link\">Dashboard</a><a href=\"#\" class=\"nav-link\">Profile</a><a href=\"#\" class=\"nav-link\">Settings</a></nav></div>",
                        Css = ".sidebar { width: 250px; height: 100vh; background: #2c3e50; color: white; position: fixed; left: 0; top: 0; } .sidebar-header { padding: 20px; border-bottom: 1px solid #34495e; font-size: 18px; font-weight: bold; } .sidebar-nav { padding: 20px 0; } .nav-link { display: block; padding: 15px 20px; color: white; text-decoration: none; transition: background 0.3s ease; } .nav-link:hover { background: #34495e; }",
                        Js = "// Toggle sidebar logic here",
                        PreviewUrl = "https://example.com/sidebar.png",
                        Type = "component",
                        Framework = "HTML/CSS/JS",
                        Price = 0,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
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
                    new Category { Name = "Buttons", Description = "All types of buttons and interactive elements" },
                    new Category { Name = "Forms", Description = "Form components, inputs, and validation elements" },
                    new Category { Name = "Navigation", Description = "Navigation bars, menus, and breadcrumbs" },
                    new Category { Name = "Layout", Description = "Grid systems, containers, and layout components" },
                    new Category { Name = "Feedback", Description = "Alerts, notifications, and status indicators" }
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
                    new Tag { Name = "dark-mode" },
                    new Tag { Name = "bootstrap" },
                    new Tag { Name = "material-design" }
                };
                await context.Tags.AddRangeAsync(tags);
            }
        }
    }
} 