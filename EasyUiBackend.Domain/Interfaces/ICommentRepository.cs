using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetByComponentIdAsync(Guid componentId);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<Comment> AddAsync(Comment comment);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(Guid id);
    }
} 