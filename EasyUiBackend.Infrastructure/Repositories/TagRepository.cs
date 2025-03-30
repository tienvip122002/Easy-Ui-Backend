using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Persistence;

namespace EasyUiBackend.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags
                .Include(t => t.Creator)
                .Include(t => t.Updater)
                .Include(t => t.Components)
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
            return await _context.Tags
                .Include(t => t.Creator)
                .Include(t => t.Updater)
                .Include(t => t.Components)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
        }

        public async Task<Tag> AddAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task UpdateAsync(Tag tag)
        {
            tag.UpdatedAt = DateTime.UtcNow;
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                tag.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }
} 