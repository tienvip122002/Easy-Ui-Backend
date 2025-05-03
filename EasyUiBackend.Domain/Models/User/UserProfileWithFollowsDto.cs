using System;

namespace EasyUiBackend.Domain.Models.User
{
    public class UserProfileWithFollowsDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Follow information
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowedByCurrentUser { get; set; }
    }
} 