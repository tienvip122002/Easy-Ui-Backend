using System;

namespace EasyUiBackend.Domain.Models.User
{
    public class UserFollowDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public DateTime FollowedAt { get; set; }
    }
} 