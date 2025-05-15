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
		public DbSet<AboutUs> AboutUs { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<ComponentLike> ComponentLikes { get; set; }
		public DbSet<UserFollow> UserFollows { get; set; }
		public DbSet<Article> Articles { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }

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

			// Configure ComponentLike relationships
			builder.Entity<ComponentLike>()
				.HasKey(cl => new { cl.UserId, cl.UIComponentId });

			builder.Entity<ComponentLike>()
				.HasOne(cl => cl.User)
				.WithMany(u => u.LikedComponents)
				.HasForeignKey(cl => cl.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<ComponentLike>()
				.HasOne(cl => cl.UIComponent)
				.WithMany(c => c.Likes)
				.HasForeignKey(cl => cl.UIComponentId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<UIComponent>()
				.HasMany(c => c.LikedByUsers)
				.WithMany(u => u.LikedUIComponents)
				.UsingEntity<ComponentLike>();

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

			builder.Entity<Order>()
				.HasOne(o => o.User)
				.WithMany()
				.HasForeignKey(o => o.UserId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<OrderItem>()
				.HasOne(oi => oi.Order)
				.WithMany(o => o.Items)
				.HasForeignKey(oi => oi.OrderId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Payment>()
				.HasOne(p => p.Order)
				.WithMany()
				.HasForeignKey(p => p.OrderId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure UserFollow relationships
			builder.Entity<UserFollow>()
				.HasKey(uf => new { uf.FollowerId, uf.FollowedId });

			builder.Entity<UserFollow>()
				.HasOne(uf => uf.Follower)
				.WithMany(u => u.Following)
				.HasForeignKey(uf => uf.FollowerId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<UserFollow>()
				.HasOne(uf => uf.Followed)
				.WithMany(u => u.Followers)
				.HasForeignKey(uf => uf.FollowedId)
				.OnDelete(DeleteBehavior.Restrict);

			// Configure Article relationships
			builder.Entity<Article>()
				.HasOne(a => a.Author)
				.WithMany(u => u.AuthoredArticles)
				.HasForeignKey(a => a.AuthorId)
				.OnDelete(DeleteBehavior.SetNull);

			// Configure RefreshToken relationships
			builder.Entity<RefreshToken>()
				.HasOne(rt => rt.User)
				.WithMany(u => u.RefreshTokens)
				.HasForeignKey(rt => rt.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
