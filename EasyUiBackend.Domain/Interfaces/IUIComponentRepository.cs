using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;

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