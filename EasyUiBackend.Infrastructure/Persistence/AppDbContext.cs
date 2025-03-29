using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EasyUiBackend.Infrastructure.Persistence
{
	public class AppDbContext : IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<UIComponent> UIComponents => Set<UIComponent>();

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Cấu hình tên bảng Identity
			builder.Entity<ApplicationUser>(entity =>
			{
				entity.ToTable("AspNetUsers");
			});

			builder.Entity<IdentityRole>(entity =>
			{
				entity.ToTable("AspNetRoles");
			});

			builder.Entity<IdentityUserRole<string>>(entity =>
			{
				entity.ToTable("AspNetUserRoles");
			});

			builder.Entity<IdentityUserClaim<string>>(entity =>
			{
				entity.ToTable("AspNetUserClaims");
			});

			builder.Entity<IdentityUserLogin<string>>(entity =>
			{
				entity.ToTable("AspNetUserLogins");
			});

			builder.Entity<IdentityRoleClaim<string>>(entity =>
			{
				entity.ToTable("AspNetRoleClaims");
			});

			builder.Entity<IdentityUserToken<string>>(entity =>
			{
				entity.ToTable("AspNetUserTokens");
			});

			// Cấu hình cho UIComponent
			builder.Entity<UIComponent>(entity =>
			{
				entity.ToTable("UIComponents");
				entity.HasKey(e => e.Id);
				entity.Property(e => e.Name).IsRequired();
			});
		}
	}
}
