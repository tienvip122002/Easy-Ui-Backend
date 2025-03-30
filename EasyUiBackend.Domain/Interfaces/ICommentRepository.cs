using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllAsync();
        Task<IEnumerable<Comment>> GetByComponentIdAsync(Guid componentId);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<Comment> AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(Guid id);
    }
} 