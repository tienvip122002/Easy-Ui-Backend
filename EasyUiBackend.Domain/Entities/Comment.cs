namespace EasyUiBackend.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = null!;
        
        // Component relationship
        public Guid ComponentId { get; set; }
        public virtual UIComponent Component { get; set; }
        
        // Navigation properties
        public virtual ApplicationUser? Creator { get; set; }
        public virtual ApplicationUser? Updater { get; set; }
    }
} 