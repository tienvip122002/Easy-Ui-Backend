using System;

namespace EasyUiBackend.Domain.Entities
{
    /// <summary>
    /// Represents a like relationship between a user and a UI component.
    /// </summary>
    public class ComponentLike
    {
        /// <summary>
        /// The ID of the user who liked the component.
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Reference to the user who liked the component.
        /// </summary>
        public virtual ApplicationUser User { get; set; }
        
        /// <summary>
        /// The ID of the UI component that was liked.
        /// </summary>
        public Guid UIComponentId { get; set; }
        
        /// <summary>
        /// Reference to the UI component that was liked.
        /// </summary>
        public virtual UIComponent UIComponent { get; set; }
        
        /// <summary>
        /// The date and time when the component was liked.
        /// </summary>
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}
