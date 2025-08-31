using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.Auth;
using EasyUiBackend.Api.Extensions;
using System.Text.Json;

namespace EasyUiBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IIdentityService identityService, ILogger<AuthController> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation("Register attempt for email: {Email}", request.Email);
                
                // Log request details (without sensitive data)
                _logger.LogDebug("Register request - Email: {Email}, FullName: {FullName}", 
                    request.Email, request.FullName);
                
                var result = await _identityService.RegisterAsync(request);
                
                _logger.LogInformation("Registration successful for email: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for email: {Email}. Error: {ErrorMessage}", 
                    request?.Email, ex.Message);
                
                // Return detailed error information for debugging
                return BadRequest(new { 
                    Message = ex.Message,
                    ErrorType = ex.GetType().Name,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    RequestData = new {
                        Email = request?.Email,
                        FullName = request?.FullName,
                        HasPassword = !string.IsNullOrEmpty(request?.Password)
                    }
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);
                
                var result = await _identityService.LoginAsync(request);
                
                _logger.LogInformation("Login successful for email: {Email}", request.Email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email: {Email}. Error: {ErrorMessage}", 
                    request?.Email, ex.Message);
                
                return BadRequest(new { 
                    Message = ex.Message,
                    ErrorType = ex.GetType().Name,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    RequestData = new {
                        Email = request?.Email,
                        HasPassword = !string.IsNullOrEmpty(request?.Password)
                    }
                });
            }
        }

        [HttpPost("google-login")]
        public async Task<ActionResult<AuthResponse>> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                var result = await _identityService.GoogleLoginAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _identityService.RefreshTokenAsync(request.RefreshToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<ActionResult> RevokeToken()
        {
            try
            {
                var userId = User.GetUserId().ToString();
                var result = await _identityService.RevokeTokenAsync(userId);
                
                if (result)
                    return Ok(new { Message = "Tokens revoked successfully" });
                else
                    return BadRequest(new { Message = "Failed to revoke tokens" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            try
            {
                var userId = User.GetUserId();
                var profile = await _identityService.GetUserProfileAsync(userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get profile failed for user: {UserId}. Error: {ErrorMessage}", 
                    User.GetUserId(), ex.Message);
                
                return BadRequest(new { 
                    Message = ex.Message,
                    ErrorType = ex.GetType().Name,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("health")]
        public async Task<ActionResult> HealthCheck()
        {
            try
            {
                _logger.LogInformation("Health check requested");
                
                // Test database connection by checking if we can access users
                var healthStatus = await _identityService.HealthCheckAsync();
                
                return Ok(new { 
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    DatabaseConnection = healthStatus,
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed. Error: {ErrorMessage}", ex.Message);
                
                return StatusCode(500, new { 
                    Status = "Unhealthy",
                    Message = ex.Message,
                    ErrorType = ex.GetType().Name,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
} 