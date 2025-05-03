using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.UIComponent;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface IUIComponentRepository
    {
        Task<IEnumerable<UIComponent>> GetAllAsync(string includeProperties = "");
        Task<(IEnumerable<UIComponent> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string includeProperties = "");
        Task<UIComponent> GetByIdAsync(Guid id, string includeProperties = "");
        Task<UIComponent> AddAsync(UIComponent entity);
        Task UpdateAsync(UIComponent entity);
        Task DeleteAsync(Guid id);
        Task<(IEnumerable<UIComponent> Items, int TotalCount)> SearchAsync(SearchUIComponentRequest request);
        Task<(IEnumerable<UIComponent> Items, int TotalCount)> FilterAsync(FilterUIComponentRequest request);
        
        // Like functionality
        Task<bool> LikeComponentAsync(Guid componentId, Guid userId);
        Task<bool> UnlikeComponentAsync(Guid componentId, Guid userId);
        Task<bool> IsLikedByUserAsync(Guid componentId, Guid userId);
        Task<IEnumerable<ComponentLike>> GetComponentLikesAsync(Guid componentId);
        Task<IEnumerable<UIComponent>> GetUserLikedComponentsAsync(Guid userId, string includeProperties = "");
        Task<ICollection<Guid>> GetLikedComponentIdsByUserAsync(Guid userId, ICollection<Guid> componentIds);
        
        // View tracking
        Task IncrementViewCountAsync(Guid componentId);
    }
} 