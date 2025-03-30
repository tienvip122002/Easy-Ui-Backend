namespace EasyUiBackend.Domain.Entities
{
	public class UIComponent : BaseEntity
	{
		public required string Name { get; set; }
		public string? Description { get; set; }
		public required string Code { get; set; }
		public string? PreviewUrl { get; set; }
		public string? Type { get; set; }
		public string? Framework { get; set; }

		// Navigation properties
		public virtual ApplicationUser? Creator { get; set; }
		public virtual ApplicationUser? Updater { get; set; }
		public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
		public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
		public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
		public virtual ICollection<ApplicationUser> SavedByUsers { get; set; } = new List<ApplicationUser>();
	}
}