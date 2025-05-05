using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.User;
using EasyUiBackend.Domain.Models.Common;
using EasyUiBackend.Api.Extensions;
using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Infrastructure.Persistence;
using EasyUiBackend.Domain.Models.UIComponent;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using EasyUiBackend.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using EasyUiBackend.Domain.Models.Email;

namespace EasyUiBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserFollowRepository _repository;
        private readonly IIdentityService _identityService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MailSettings _mailSettings;

        public UserController(
            IUserFollowRepository repository,
            IIdentityService identityService,
            AppDbContext context,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IOptions<MailSettings> mailSettings)
        {
            _repository = repository;
            _identityService = identityService;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _mailSettings = mailSettings.Value;
        }

        [HttpGet("{id}/profile")]
        public async Task<ActionResult<UserProfileWithFollowsDto>> GetUserProfile(Guid id)
        {
            Guid? currentUserId = null;
            if (User.Identity.IsAuthenticated)
            {
                currentUserId = User.GetUserId();
            }

            var profile = await _repository.GetUserProfileWithFollowsAsync(id, currentUserId);
            
            if (profile == null)
                return NotFound("User not found");

            return Ok(profile);
        }

        [HttpPost("{id}/follow")]
        public async Task<IActionResult> FollowUser(Guid id)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var currentUserId = User.GetUserId();
            
            // Can't follow yourself
            if (currentUserId == id)
                return BadRequest("Cannot follow yourself");

            var result = await _repository.FollowUserAsync(currentUserId, id);

            if (!result)
                return NotFound("User not found");

            return NoContent();
        }

        [HttpPost("{id}/unfollow")]
        public async Task<IActionResult> UnfollowUser(Guid id)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var currentUserId = User.GetUserId();
            var result = await _repository.UnfollowUserAsync(currentUserId, id);

            if (!result)
                return NotFound("User or follow relationship not found");

            return NoContent();
        }

        [HttpGet("{id}/followers")]
        public async Task<ActionResult<PaginatedResponse<UserFollowDto>>> GetFollowers(
            Guid id, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            // Check if user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == id);
            if (!userExists)
                return NotFound("User not found");

            var followers = await _repository.GetFollowersAsync(id, pageNumber, pageSize);
            return Ok(followers);
        }

        [HttpGet("{id}/following")]
        public async Task<ActionResult<PaginatedResponse<UserFollowDto>>> GetFollowing(
            Guid id, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            // Check if user exists
            var userExists = await _context.Users.AnyAsync(u => u.Id == id);
            if (!userExists)
                return NotFound("User not found");

            var following = await _repository.GetFollowingAsync(id, pageNumber, pageSize);
            return Ok(following);
        }

        [HttpGet("{id}/components")]
        public async Task<ActionResult<PaginatedResponse<UIComponentListDto>>> GetUserComponents(
            Guid id, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            // Kiểm tra user tồn tại
            var userExists = await _context.Users.AnyAsync(u => u.Id == id);
            if (!userExists)
                return NotFound("User not found");

            // Query các component của user này
            var query = _context.UIComponents
                .AsNoTracking()  // Tối ưu query bằng cách không tracking
                .Where(c => c.CreatedBy == id && c.IsActive)  // Chỉ lấy component active
                .Include(c => c.Categories)
                .Include(c => c.Tags);
                
            // Đếm tổng số
            var totalCount = await query.CountAsync();
            
            // Phân trang và sắp xếp
            var components = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            // Map qua DTOs
            var dtos = _mapper.Map<List<UIComponentListDto>>(components);
            
            // Xử lý PreviewImage từ PreviewUrl nếu cần
            foreach (var dto in dtos)
            {
                var component = components.FirstOrDefault(c => c.Id == dto.Id);
                if (component != null && string.IsNullOrEmpty(dto.PreviewImage) && !string.IsNullOrEmpty(component.PreviewUrl))
                {
                    dto.PreviewImage = component.PreviewUrl;
                }
            }
            
            // Kiểm tra liked status nếu user đã đăng nhập
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.GetUserId();
                var componentIds = dtos.Select(d => d.Id).ToList();
                
                // Lấy repository từ DI container
                var uiComponentRepository = HttpContext.RequestServices.GetService<IUIComponentRepository>();
                if (uiComponentRepository != null)
                {
                    var likedComponentIds = await uiComponentRepository.GetLikedComponentIdsByUserAsync(userId, componentIds);
                    
                    foreach (var dto in dtos)
                    {
                        dto.IsLikedByCurrentUser = likedComponentIds.Contains(dto.Id);
                    }
                }
            }
            
            // Tạo response có phân trang
            var response = new PaginatedResponse<UIComponentListDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
            
            return Ok(response);
        }

        [HttpGet("me/detail")]
        public async Task<ActionResult<UserDetailDto>> GetMyDetail()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
                
            var userId = User.GetUserId();
            return await GetUserDetailById(userId);
        }
        
        [HttpGet("{id}/detail")]
        public async Task<ActionResult<UserDetailDto>> GetUserDetail(Guid id)
        {
            return await GetUserDetailById(id);
        }
        
        private async Task<ActionResult<UserDetailDto>> GetUserDetailById(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user == null)
                return NotFound("User not found");
                
            var userDetail = _mapper.Map<UserDetailDto>(user);
            
            // Kiểm tra trạng thái follow
            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = User.GetUserId();
                userDetail.IsFollowedByCurrentUser = await _repository.IsFollowingAsync(currentUserId, userId);
            }
            
            // Đảm bảo mảng rỗng thay vì null để tránh lỗi ở FE
            if (userDetail.WorkHistory == null)
                userDetail.WorkHistory = new List<WorkHistoryItem>();
            
            if (userDetail.Education == null)
                userDetail.Education = new List<EducationItem>();
            
            return Ok(userDetail);
        }
        
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateUserProfileRequest request)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
                
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user == null)
                return NotFound("User not found");
                
            // Kiểm tra và giới hạn số lượng WorkHistory và Education
            if (request.WorkHistory != null && request.WorkHistory.Count > 5)
            {
                request.WorkHistory = request.WorkHistory.Take(5).ToList();
                ModelState.AddModelError("WorkHistory", "Chỉ được phép tối đa 5 mục kinh nghiệm làm việc");
            }
            
            if (request.Education != null && request.Education.Count > 5)
            {
                request.Education = request.Education.Take(5).ToList();
                ModelState.AddModelError("Education", "Chỉ được phép tối đa 5 mục học vấn");
            }
            
            // Cập nhật thông tin
            _mapper.Map(request, user);
            
            // Cập nhật số điện thoại
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                var phoneNumberResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
                if (!phoneNumberResult.Succeeded)
                {
                    ModelState.AddModelError("PhoneNumber", "Không thể cập nhật số điện thoại");
                    return BadRequest(ModelState);
                }
            }
            
            // Lưu các thay đổi khác
            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded)
                return NoContent();
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            
            return BadRequest(ModelState);
        }
        
        // Endpoint riêng để cập nhật avatar (sử dụng form data)
        [HttpPost("me/avatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
                
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user == null)
                return NotFound("User not found");
                
            // Xử lý upload file avatar ở đây
            // Đây chỉ là giải pháp tạm thời, bạn cần triển khai code upload file thực tế
            if (file != null && file.Length > 0)
            {
                // Giả định có service lưu file và trả về URL
                // string avatarUrl = await _fileService.UploadUserAvatar(file);
                // user.Avatar = avatarUrl;
                
                // Tạm thời cài đặt mock
                user.Avatar = $"/avatars/{userId}.jpg"; 
                
                var result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                    return Ok(new { avatarUrl = user.Avatar });
                    
                return BadRequest("Không thể cập nhật avatar");
            }
            
            return BadRequest("Không có file avatar được tải lên");
        }

        [HttpGet("me/email-status")]
        public async Task<IActionResult> GetEmailConfirmationStatus()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
                
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user == null)
                return NotFound("User not found");
                
            return Ok(new { 
                email = user.Email,
                isEmailConfirmed = user.EmailConfirmed 
            });
        }

        [HttpPost("me/send-confirmation-email")]
        public async Task<IActionResult> SendConfirmationEmail()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
                
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user == null)
                return NotFound("User not found");
                
            if (user.EmailConfirmed)
                return BadRequest("Email is already confirmed");
                
            // Generate the email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            
            // Encode the token for URL safety
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            
            // Build the confirmation link - Important: Update this URL to match your frontend URL
            var confirmationLink = $"http://localhost:3000/verify-email?userId={user.Id}&token={token}";
            
            try
            {
                // Create mail message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_mailSettings.DefaultSender, _mailSettings.DisplayName),
                    Subject = "Confirm your email",
                    Body = $@"
                        <html>
                        <head>
                            <style>
                                body {{ font-family: Arial, sans-serif; }}
                                .container {{ padding: 20px; max-width: 600px; margin: 0 auto; }}
                                .btn {{ background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <h2>Hello {user.UserName},</h2>
                                <p>Thank you for registering! Please confirm your email address by clicking the button below:</p>
                                <p><a href='{confirmationLink}' class='btn'>Confirm Email</a></p>
                                <p>Or copy and paste the following link in your browser:</p>
                                <p>{confirmationLink}</p>
                                <p>If you did not request this email, please ignore it.</p>
                            </div>
                        </body>
                        </html>
                    ",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(user.Email);

                // Configure SMTP client
                var smtpClient = new SmtpClient(_mailSettings.Provider)
                {
                    Port = _mailSettings.Port,
                    Credentials = new NetworkCredential(_mailSettings.DefaultSender, _mailSettings.Password),
                    EnableSsl = true
                };
                
                // Send email
                await smtpClient.SendMailAsync(mailMessage);
                
                return Ok(new { message = "Confirmation email sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to send confirmation email", details = ex.Message });
            }
        }

        // New endpoint to confirm email
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("User ID and token are required");
                
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");
                
            // Decode the token from Base64URL
            try
            {
                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch (Exception)
            {
                return BadRequest("Invalid token format");
            }
            
            // Confirm the email
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Email successfully confirmed - you can redirect to a success page
                return Ok(new { message = "Email confirmed successfully" });
            }
            
            // If we got this far, something failed
            return BadRequest(new { message = "Email confirmation failed", errors = result.Errors.Select(e => e.Description) });
        }
    }
} 