namespace EasyUiBackend.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public required string Content { get; set; }
        public Guid ComponentId { get; set; }

        // Navigation properties
        public virtual UIComponent Component { get; set; } = null!;
        public virtual ApplicationUser? Creator { get; set; }
        public virtual ApplicationUser? Updater { get; set; }
    }
} 