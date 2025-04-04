using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface IUIComponentRepository
    {
        Task<IEnumerable<UIComponent>> GetAllAsync(string includeProperties = "");
        Task<UIComponent> GetByIdAsync(Guid id, string includeProperties = "");
        Task<UIComponent> AddAsync(UIComponent entity);
        Task UpdateAsync(UIComponent entity);
        Task DeleteAsync(Guid id);
    }
} 