using Microsoft.AspNetCore.Identity;

namespace EasyUiBackend.Domain.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        // Thêm các thuộc tính tùy chỉnh nếu cần
        public string? Description { get; set; }
    }
} 