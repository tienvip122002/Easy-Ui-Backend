using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Common;
using EasyUiBackend.Domain.Models.User;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface IUserFollowRepository
    {
        // Follow/unfollow functionality
        Task<bool> FollowUserAsync(Guid followerId, Guid followedId);
        Task<bool> UnfollowUserAsync(Guid followerId, Guid followedId);
        Task<bool> IsFollowingAsync(Guid followerId, Guid followedId);
        
        // Get followers/following
        Task<PaginatedResponse<UserFollowDto>> GetFollowersAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
        Task<PaginatedResponse<UserFollowDto>> GetFollowingAsync(Guid userId, int pageNumber = 1, int pageSize = 10);
        
        // Get user profile with follow information
        Task<UserProfileWithFollowsDto> GetUserProfileWithFollowsAsync(Guid userId, Guid? currentUserId = null);
        
        // Get IDs of users followed by the current user from a list
        Task<ICollection<Guid>> GetFollowedUserIdsByUserAsync(Guid userId, ICollection<Guid> userIds);
    }
} 