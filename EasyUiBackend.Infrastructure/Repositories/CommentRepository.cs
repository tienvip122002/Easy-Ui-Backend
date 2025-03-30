using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Persistence;

namespace EasyUiBackend.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await _context.Comments
                .Include(c => c.Creator)
                .Include(c => c.Updater)
                .Include(c => c.Component)
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetByComponentIdAsync(Guid componentId)
        {
            return await _context.Comments
                .Include(c => c.Creator)
                .Include(c => c.Updater)
                .Where(c => c.ComponentId == componentId && c.IsActive)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _context.Comments
                .Include(c => c.Creator)
                .Include(c => c.Updater)
                .Include(c => c.Component)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<Comment> AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task UpdateAsync(Comment comment)
        {
            comment.UpdatedAt = DateTime.UtcNow;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }
} 