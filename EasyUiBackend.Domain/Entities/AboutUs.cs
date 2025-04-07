namespace EasyUiBackend.Domain.Entities;

public class AboutUs : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser? Creator { get; set; }
    public virtual ApplicationUser? Updater { get; set; }
} 