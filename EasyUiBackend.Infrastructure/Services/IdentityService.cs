using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.Auth;
using AutoMapper;
using Google.Apis.Auth;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Infrastructure.Persistence;

namespace EasyUiBackend.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMapper mapper,
            AppDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new Exception("User, Email not found");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
                throw new Exception("Invalid password");

            var token = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(user, refreshToken);

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"]))
            };
        }

        public async Task<AuthResponse> GoogleLoginAsync(GoogleLoginRequest request)
        {
            try
            {
                // Validate the Google token
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                };
                
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.GoogleToken, settings);
                
                // Check if user exists with this email
                var user = await _userManager.FindByEmailAsync(payload.Email);
                
                if (user == null)
                {
                    // Create new user if they don't exist
                    user = new ApplicationUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email.Split('@')[0], // Use email as username but remove domain
                        FullName = payload.Name,
                        Avatar = payload.Picture,
                        EmailConfirmed = true, // Google already confirmed the email
                        CreatedAt = DateTime.UtcNow
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        throw new Exception($"Failed to create user from Google account: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    
                    // You could add the user to a "Google" role if needed
                    // await _userManager.AddToRoleAsync(user, "GoogleUser");
                }
                else 
                {
                    // Update existing user information from Google if needed
                    bool needsUpdate = false;
                    
                    if (string.IsNullOrEmpty(user.FullName) && !string.IsNullOrEmpty(payload.Name))
                    {
                        user.FullName = payload.Name;
                        needsUpdate = true;
                    }
                    
                    if (string.IsNullOrEmpty(user.Avatar) && !string.IsNullOrEmpty(payload.Picture))
                    {
                        user.Avatar = payload.Picture;
                        needsUpdate = true;
                    }
                    
                    if (needsUpdate)
                    {
                        await _userManager.UpdateAsync(user);
                    }
                }

                // Generate JWT token
                var token = await GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();
                await SaveRefreshTokenAsync(user, refreshToken);
                
                return new AuthResponse
                {
                    Token = token,
                    RefreshToken = refreshToken.Token,
                    Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"]))
                };
            }
            catch (InvalidJwtException ex)
            {
                throw new Exception($"Invalid Google token: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Google authentication failed: {ex.Message}");
            }
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
                throw new Exception("User, Email already exists");

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FullName = request.FullName,
                Avatar = request.Avatar,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new Exception($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            var token = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshTokenAsync(user, refreshToken);

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"]))
            };
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiryTime = DateTime.UtcNow.AddDays(7), // Refresh token valid for 7 days
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task SaveRefreshTokenAsync(ApplicationUser user, RefreshToken refreshToken)
        {
            // Set JwtId and UserId
            refreshToken.JwtId = Guid.NewGuid().ToString();
            refreshToken.UserId = user.Id;
            
            // Add to context
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null)
                throw new Exception("Invalid refresh token");

            if (storedToken.ExpiryTime < DateTime.UtcNow)
                throw new Exception("Refresh token expired");

            if (storedToken.IsRevoked)
                throw new Exception("Refresh token revoked");

            if (storedToken.IsUsed)
                throw new Exception("Refresh token has been used");

            // Update current token
            storedToken.IsUsed = true;
            _context.RefreshTokens.Update(storedToken);
            
            // Generate new tokens
            var user = storedToken.User;
            var newJwtToken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            
            // Save new refresh token
            await SaveRefreshTokenAsync(user, newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken.Token,
                Expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"]))
            };
        }

        public async Task<bool> RevokeTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            // Mark all refresh tokens for this user as revoked
            var refreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == Guid.Parse(userId) && !rt.IsRevoked)
                .ToListAsync();

            if (!refreshTokens.Any())
                return false;

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found");

            var roles = await _userManager.GetRolesAsync(user);
            
            var profile = _mapper.Map<UserProfileDto>(user);
            profile.Roles = roles.ToList();
            
            return profile;
        }
    }
} 