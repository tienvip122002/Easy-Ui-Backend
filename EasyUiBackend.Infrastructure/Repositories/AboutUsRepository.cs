using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.AboutUs;
using EasyUiBackend.Infrastructure.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace EasyUiBackend.Infrastructure.Repositories;

public class AboutUsRepository : IAboutUsRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AboutUsRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AboutUs>> GetAllAsync()
    {
        return await _context.AboutUs
            .Where(x => x.IsActive)
            .Include(x => x.Creator)
            .Include(x => x.Updater)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<AboutUs?> GetByIdAsync(Guid id)
    {
        return await _context.AboutUs
            .Include(x => x.Creator)
            .Include(x => x.Updater)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
    }

    public async Task<AboutUs> AddAsync(AboutUs aboutUs)
    {
        await _context.AboutUs.AddAsync(aboutUs);
        await _context.SaveChangesAsync();
        return _mapper.Map<AboutUs>(aboutUs);
    }

    public async Task UpdateAsync(AboutUs aboutUs)
    {
        aboutUs.UpdatedAt = DateTime.UtcNow;
        _context.AboutUs.Update(aboutUs);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var aboutUs = await _context.AboutUs.FindAsync(id);
        if (aboutUs != null)
        {
            aboutUs.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<AboutUs>> SearchAsync(SearchAboutUsRequest request)
    {
        IQueryable<AboutUs> query = _context.AboutUs
            .AsNoTracking()
            .Where(x => x.IsActive);

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            var keyword = request.Keyword.ToLower();
            query = query.Where(x => 
                EF.Functions.Like(x.Title.ToLower(), $"%{keyword}%") || 
                EF.Functions.Like(x.Content.ToLower(), $"%{keyword}%")
            );
        }

        return await query
            .Include(x => x.Creator)
            .Include(x => x.Updater)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
} 