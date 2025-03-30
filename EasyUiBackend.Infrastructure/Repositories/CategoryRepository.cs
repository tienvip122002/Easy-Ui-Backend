using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Persistence;

namespace EasyUiBackend.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Creator)
                .Include(c => c.Updater)
                .Include(c => c.Components)
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                .Include(c => c.Creator)
                .Include(c => c.Updater)
                .Include(c => c.Components)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<Category> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task UpdateAsync(Category category)
        {
            category.UpdatedAt = DateTime.UtcNow;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }
} 