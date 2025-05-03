using System;

namespace EasyUiBackend.Domain.Entities
{
    /// <summary>
    /// Represents a follow relationship between users.
    /// </summary>
    public class UserFollow
    {
        /// <summary>
        /// The ID of the follower user (the one who is following).
        /// </summary>
        public Guid FollowerId { get; set; }
        
        /// <summary>
        /// Reference to the follower user.
        /// </summary>
        public virtual ApplicationUser Follower { get; set; }
        
        /// <summary>
        /// The ID of the user being followed.
        /// </summary>
        public Guid FollowedId { get; set; }
        
        /// <summary>
        /// Reference to the user being followed.
        /// </summary>
        public virtual ApplicationUser Followed { get; set; }
        
        /// <summary>
        /// The date and time when the follow relationship was created.
        /// </summary>
        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
} 