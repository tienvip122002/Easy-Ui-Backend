using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using easyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Infrastructure.Persistence
{
	public class AppDbContext : IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<UIComponent> UIComponents => Set<UIComponent>();
	}
}
