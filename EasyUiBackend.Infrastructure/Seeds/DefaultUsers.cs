using Microsoft.AspNetCore.Identity;
using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Infrastructure.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                FullName = "Admin User",
                EmailConfirmed = true
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Admin@123");
                    await userManager.AddToRoleAsync(defaultUser, "Admin");
                }
            }
        }
    }
} 