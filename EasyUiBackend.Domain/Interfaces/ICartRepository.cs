using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetUserCartAsync(Guid userId);
        Task<Cart?> GetByIdAsync(Guid id);
        Task<Cart> AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Guid id);
        Task<Cart?> GetUserCartItemAsync(Guid userId, Guid componentId);
    }
} 