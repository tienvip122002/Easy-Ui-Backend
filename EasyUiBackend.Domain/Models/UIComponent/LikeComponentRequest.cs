using System;

namespace EasyUiBackend.Domain.Models.UIComponent
{
    /// <summary>
    /// Request model for liking or unliking a UI component.
    /// </summary>
    public class LikeComponentRequest
    {
        /// <summary>
        /// The ID of the UI component to like/unlike.
        /// Not required in route parameters.
        /// </summary>
        public Guid? ComponentId { get; set; }
    }
}
