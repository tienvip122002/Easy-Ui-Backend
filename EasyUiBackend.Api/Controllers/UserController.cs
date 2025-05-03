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

        public UserController(
            IUserFollowRepository repository,
            IIdentityService identityService,
            AppDbContext context,
            IMapper mapper)
        {
            _repository = repository;
            _identityService = identityService;
            _context = context;
            _mapper = mapper;
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
    }
} 