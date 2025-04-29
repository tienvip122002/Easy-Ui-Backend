using System;

namespace EasyUiBackend.Domain.Models.UIComponent
{
    /// <summary>
    /// Data transfer object for a component like.
    /// </summary>
    public class ComponentLikeDto
    {
        /// <summary>
        /// The ID of the user who liked the component.
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// The username of the user who liked the component.
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// The ID of the UI component that was liked.
        /// </summary>
        public Guid UIComponentId { get; set; }
        
        /// <summary>
        /// The date and time when the component was liked.
        /// </summary>
        public DateTime LikedAt { get; set; }
    }
}
