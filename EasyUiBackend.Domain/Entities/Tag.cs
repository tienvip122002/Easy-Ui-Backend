namespace EasyUiBackend.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public required string Name { get; set; }

        // Navigation properties
        public virtual ApplicationUser? Creator { get; set; }
        public virtual ApplicationUser? Updater { get; set; }
        public virtual ICollection<UIComponent> Components { get; set; } = new List<UIComponent>();
    }
} 