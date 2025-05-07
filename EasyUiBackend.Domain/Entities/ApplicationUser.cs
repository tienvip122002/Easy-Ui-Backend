using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace EasyUiBackend.Domain.Entities
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		// Thông tin cá nhân
		public string? FullName { get; set; }
		public string? Avatar { get; set; }
		public string? Location { get; set; }
		public string? Bio { get; set; }
		public string? Website { get; set; }
		public string? WorkDisplayEmail { get; set; }
		
		// Thời gian
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public bool IsActive { get; set; } = true;

		// Navigation properties
		public virtual ICollection<UIComponent> CreatedComponents { get; set; } = new List<UIComponent>();
		public virtual ICollection<UIComponent> UpdatedComponents { get; set; } = new List<UIComponent>();
		public virtual ICollection<UIComponent> SavedComponents { get; set; } = new List<UIComponent>();
		public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
		
		// Navigation properties for created/updated entities
		public virtual ICollection<Category> CreatedCategories { get; set; } = new List<Category>();
		public virtual ICollection<Category> UpdatedCategories { get; set; } = new List<Category>();
		public virtual ICollection<Tag> CreatedTags { get; set; } = new List<Tag>();
		public virtual ICollection<Tag> UpdatedTags { get; set; } = new List<Tag>();
		[JsonIgnore]
		public virtual ICollection<Comment> CreatedComments { get; set; } = new List<Comment>();
		public virtual ICollection<Comment> UpdatedComments { get; set; } = new List<Comment>();

		// Articles authored by this user
		public virtual ICollection<Article> AuthoredArticles { get; set; } = new List<Article>();

		// Add this property to your ApplicationUser class
		public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();

		// Components that the user has liked
		public virtual ICollection<ComponentLike> LikedComponents { get; set; } = new List<ComponentLike>();
		public virtual ICollection<UIComponent> LikedUIComponents { get; set; } = new List<UIComponent>();

		// User follow relationships
		// Users that are following this user
		[JsonIgnore]
		public virtual ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
		
		// Users that this user is following
		[JsonIgnore]
		public virtual ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
		
		// Count of followers (for quick reference)
		public int FollowersCount { get; set; }
		
		// Count of following (for quick reference)
		public int FollowingCount { get; set; }
		
		// Work history và education sẽ được lưu dưới dạng JSON string
		public string? WorkHistory { get; set; }
		public string? Education { get; set; }
	}
}
