using System;
using System.ComponentModel.DataAnnotations;

namespace EasyUiBackend.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }
        public string JwtId { get; set; }
        
        // Liên kết với User
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
} 