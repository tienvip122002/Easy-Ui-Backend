namespace EasyUiBackend.Domain.Models.Auth
{
    public class RegisterRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
} 