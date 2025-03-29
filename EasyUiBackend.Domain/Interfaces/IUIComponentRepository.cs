using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface IUIComponentRepository
    {
        Task<IEnumerable<UIComponent>> GetAllAsync();
        Task<UIComponent?> GetByIdAsync(Guid id);
        Task<UIComponent> AddAsync(UIComponent entity);
        Task UpdateAsync(UIComponent entity);
        Task DeleteAsync(Guid id);
    }
} 