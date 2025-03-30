namespace EasyUiBackend.Domain.Entities
{
public class Comment : BaseEntity
{
    public string Content { get; set; } = null!;
    public Guid ComponentId { get; set; }
    
    // Navigation properties
    public virtual UIComponent Component { get; set; } = null!;
        public virtual ApplicationUser? Creator { get; set; }
        public virtual ApplicationUser? Updater { get; set; }
    }
} 