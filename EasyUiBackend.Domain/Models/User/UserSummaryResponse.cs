using System;

namespace EasyUiBackend.Domain.Models.User
{
    public class UserSummaryResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
    }
} 