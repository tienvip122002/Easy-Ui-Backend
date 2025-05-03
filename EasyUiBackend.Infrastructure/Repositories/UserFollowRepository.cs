using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.Common;
using EasyUiBackend.Domain.Models.User;
using EasyUiBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace EasyUiBackend.Infrastructure.Repositories
{
    public class UserFollowRepository : IUserFollowRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserFollowRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> FollowUserAsync(Guid followerId, Guid followedId)
        {
            // Check if users exist
            var followerExists = await _context.Users.AnyAsync(u => u.Id == followerId);
            var followedExists = await _context.Users.AnyAsync(u => u.Id == followedId);

            if (!followerExists || !followedExists)
                return false;

            // Check if already following
            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowedId == followedId);

            if (existingFollow != null)
                return true; // Already following

            // Create new follow relationship
            var follow = new UserFollow
            {
                FollowerId = followerId,
                FollowedId = followedId,
                FollowedAt = DateTime.UtcNow
            };

            _context.UserFollows.Add(follow);

            // Increment follower count on followed user
            var followedUser = await _context.Users.FindAsync(followedId);
            if (followedUser != null)
            {
                followedUser.FollowersCount += 1;
            }

            // Increment following count on follower user
            var followerUser = await _context.Users.FindAsync(followerId);
            if (followerUser != null)
            {
                followerUser.FollowingCount += 1;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnfollowUserAsync(Guid followerId, Guid followedId)
        {
            // Find the follow relationship
            var follow = await _context.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowedId == followedId);

            if (follow == null)
                return false; // Not following

            // Remove the follow relationship
            _context.UserFollows.Remove(follow);

            // Decrement follower count on followed user
            var followedUser = await _context.Users.FindAsync(followedId);
            if (followedUser != null && followedUser.FollowersCount > 0)
            {
                followedUser.FollowersCount -= 1;
            }

            // Decrement following count on follower user
            var followerUser = await _context.Users.FindAsync(followerId);
            if (followerUser != null && followerUser.FollowingCount > 0)
            {
                followerUser.FollowingCount -= 1;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFollowingAsync(Guid followerId, Guid followedId)
        {
            return await _context.UserFollows
                .AnyAsync(uf => uf.FollowerId == followerId && uf.FollowedId == followedId);
        }

        public async Task<PaginatedResponse<UserFollowDto>> GetFollowersAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.UserFollows
                .Where(uf => uf.FollowedId == userId)
                .Include(uf => uf.Follower)
                .OrderByDescending(uf => uf.FollowedAt);

            var totalCount = await query.CountAsync();
            
            var followers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = followers.Select(uf => new UserFollowDto
            {
                UserId = uf.FollowerId,
                UserName = uf.Follower.UserName,
                Email = uf.Follower.Email,
                FullName = uf.Follower.FullName,
                Avatar = uf.Follower.Avatar,
                FollowedAt = uf.FollowedAt
            }).ToList();

            return new PaginatedResponse<UserFollowDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<PaginatedResponse<UserFollowDto>> GetFollowingAsync(Guid userId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.UserFollows
                .Where(uf => uf.FollowerId == userId)
                .Include(uf => uf.Followed)
                .OrderByDescending(uf => uf.FollowedAt);

            var totalCount = await query.CountAsync();
            
            var following = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = following.Select(uf => new UserFollowDto
            {
                UserId = uf.FollowedId,
                UserName = uf.Followed.UserName,
                Email = uf.Followed.Email,
                FullName = uf.Followed.FullName,
                Avatar = uf.Followed.Avatar,
                FollowedAt = uf.FollowedAt
            }).ToList();

            return new PaginatedResponse<UserFollowDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<UserProfileWithFollowsDto> GetUserProfileWithFollowsAsync(Guid userId, Guid? currentUserId = null)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            var profile = new UserProfileWithFollowsDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Avatar = user.Avatar,
                CreatedAt = user.CreatedAt,
                FollowersCount = user.FollowersCount,
                FollowingCount = user.FollowingCount,
                IsFollowedByCurrentUser = false
            };

            // Check if followed by current user if provided
            if (currentUserId.HasValue)
            {
                profile.IsFollowedByCurrentUser = await IsFollowingAsync(currentUserId.Value, userId);
            }

            return profile;
        }

        public async Task<ICollection<Guid>> GetFollowedUserIdsByUserAsync(Guid userId, ICollection<Guid> userIds)
        {
            return await _context.UserFollows
                .Where(uf => uf.FollowerId == userId && userIds.Contains(uf.FollowedId))
                .Select(uf => uf.FollowedId)
                .ToListAsync();
        }
    }
} 