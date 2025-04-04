using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace EasyUiBackend.Domain.Entities
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		// 
		public string? FullName { get; set; }

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

		// Add this property to your ApplicationUser class
		public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
	}
}
