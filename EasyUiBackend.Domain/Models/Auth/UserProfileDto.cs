namespace EasyUiBackend.Domain.Models.Auth;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<string> Roles { get; set; } = new List<string>();
} 