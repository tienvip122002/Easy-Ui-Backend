using EasyUiBackend.Domain.Models.Auth;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string userId);
    }
} 