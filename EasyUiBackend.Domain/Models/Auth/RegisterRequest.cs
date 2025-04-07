namespace EasyUiBackend.Domain.Models.Auth
{
    public class RegisterRequest
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? PhoneNumber { get; set; }
    }
} 