using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.Category;
using EasyUiBackend.Infrastructure.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace EasyUiBackend.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CategoryRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .ProjectTo<Category>(_mapper.ConfigurationProvider)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories
            .Where(c => c.Id == id && c.IsActive)
            .ProjectTo<Category>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<Category> AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return _mapper.Map<Category>(category);
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

    public async Task<IEnumerable<Category>> SearchAsync(SearchCategoryRequest request)
    {
        IQueryable<Category> query = _context.Categories
            .AsNoTracking()
            .Where(x => x.IsActive);

        // Search by keyword in name and description (case-insensitive)
        if (!string.IsNullOrEmpty(request.Keyword))
        {
            var keyword = request.Keyword.ToLower();
            query = query.Where(x => 
                EF.Functions.Like(x.Name.ToLower(), $"%{keyword}%") || 
                (x.Description != null && EF.Functions.Like(x.Description.ToLower(), $"%{keyword}%"))
            );
        }

        return await query
            .Include(x => x.Components)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
} 