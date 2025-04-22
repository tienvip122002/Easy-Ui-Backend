namespace EasyUiBackend.Domain.Entities
{
	/// <summary>
	/// Represents a reusable UI component.
	/// </summary>
	public class UIComponent : BaseEntity
		{
			/// <summary>
			/// The display name of the component.
			/// </summary>
			/// <example>Simple Button</example>
			public required string Name { get; set; }

			/// <summary>
			/// A brief description of the component and its usage.
			/// </summary>
			/// <example>A standard button with primary styling.</example>
			public string? Description { get; set; }

			/// <summary>
			/// The HTML markup for the component.
			/// </summary>
			/// <example>&lt;button class="btn btn-primary"&gt;Click Me&lt;/button&gt;</example>
			public string? Html { get; set; }

			/// <summary>
			/// The CSS styles for the component.
			/// </summary>
			/// <example>.btn-primary { color: white; background-color: blue; }</example>
			public string? Css { get; set; }

			/// <summary>
			/// The JavaScript code for the component's functionality.
			/// </summary>
			/// <example>document.querySelector('.btn-primary').addEventListener('click', () => alert('Button clicked!'));</example>
			public string? Js { get; set; }

			/// <summary>
			/// A URL to an image or live preview of the component.
			/// </summary>
			/// <example>https://example.com/preview/simple-button.png</example>
			public string? PreviewUrl { get; set; }

			/// <summary>
			/// The type or category of the component (e.g., button, form, card).
			/// </summary>
			/// <example>Button</example>
			public string? Type { get; set; }

			/// <summary>
			/// The UI framework the component is designed for (e.g., Bootstrap, Tailwind, React).
			/// </summary>
			/// <example>Bootstrap 5</example>
			public string? Framework { get; set; }

			/// <summary>
			/// The price of the component.
			/// </summary>
			public decimal Price { get; set; }

			/// <summary>
			/// The discount price of the component.
			/// </summary>
			public decimal? DiscountPrice { get; set; }

			// Navigation properties
			public virtual ApplicationUser? Creator { get; set; }
			public virtual ApplicationUser? Updater { get; set; }
			public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
			public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
			public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
			public virtual ICollection<ApplicationUser> SavedByUsers { get; set; } = new List<ApplicationUser>();
		}
}