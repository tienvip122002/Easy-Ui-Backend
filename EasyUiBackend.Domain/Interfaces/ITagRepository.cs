using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(Guid id);
        Task<Tag> AddAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Guid id);
    }
} 