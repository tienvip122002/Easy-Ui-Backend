using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EasyUiBackend.Infrastructure.Persistence
{
	public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		public DbSet<UIComponent> UIComponents { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Cart> Carts { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Configure relationships
			builder.Entity<UIComponent>()
				.HasOne(u => u.Creator)
				.WithMany(a => a.CreatedComponents)
				.HasForeignKey(u => u.CreatedBy)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<UIComponent>()
				.HasOne(u => u.Updater)
				.WithMany(a => a.UpdatedComponents)
				.HasForeignKey(u => u.UpdatedBy)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<UIComponent>()
				.HasMany(u => u.SavedByUsers)
				.WithMany(a => a.SavedComponents)
				.UsingEntity(j => j.ToTable("SavedComponents"));

			builder.Entity<UIComponent>()
				.HasMany(u => u.Categories)
				.WithMany(c => c.Components)
				.UsingEntity(j => j.ToTable("ComponentCategories"));

			builder.Entity<UIComponent>()
				.HasMany(u => u.Tags)
				.WithMany(t => t.Components)
				.UsingEntity(j => j.ToTable("ComponentTags"));

			// Configure Category relationships
			builder.Entity<Category>()
				.HasOne(c => c.Creator)
				.WithMany(a => a.CreatedCategories)
				.HasForeignKey(c => c.CreatedBy)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Category>()
				.HasOne(c => c.Updater)
				.WithMany(a => a.UpdatedCategories)
				.HasForeignKey(c => c.UpdatedBy)
				.OnDelete(DeleteBehavior.SetNull);

			// Configure Tag relationships
			builder.Entity<Tag>()
				.HasOne(t => t.Creator)
				.WithMany(a => a.CreatedTags)
				.HasForeignKey(t => t.CreatedBy)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Tag>()
				.HasOne(t => t.Updater)
				.WithMany(a => a.UpdatedTags)
				.HasForeignKey(t => t.UpdatedBy)
				.OnDelete(DeleteBehavior.SetNull);

			// Configure Comment relationships
			builder.Entity<Comment>()
				.HasOne(c => c.Creator)
				.WithMany(a => a.CreatedComments)
				.HasForeignKey(c => c.CreatedBy)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Comment>()
				.HasOne(c => c.Updater)
				.WithMany(a => a.UpdatedComments)
				.HasForeignKey(c => c.UpdatedBy)
				.OnDelete(DeleteBehavior.SetNull);

			// Configure Cart relationships
			builder.Entity<Cart>()
				.HasOne(c => c.User)
				.WithMany()
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Cart>()
				.HasOne(c => c.UIComponent)
				.WithMany()
				.HasForeignKey(c => c.UIComponentId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
