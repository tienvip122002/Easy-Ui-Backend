using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EasyUiBackend.Domain.Models.UIComponent;

namespace EasyUiBackend.Infrastructure.Repositories;

public class UIComponentRepository : IUIComponentRepository
{
	private readonly AppDbContext _context;
	private readonly IMapper _mapper;

	public UIComponentRepository(AppDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public async Task<IEnumerable<UIComponent>> GetAllAsync(string includeProperties = "")
	{
		IQueryable<UIComponent> query = _context.UIComponents
			.AsNoTracking()
			.Where(x => x.IsActive); // Chỉ lấy các component đang active

		foreach (var includeProperty in includeProperties.Split
			(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
		{
			query = query.Include(includeProperty);
		}

		return await query
			.OrderByDescending(x => x.CreatedAt)
			.ToListAsync();
	}

	public async Task<UIComponent> GetByIdAsync(Guid id, string includeProperties = "")
	{
		IQueryable<UIComponent> query = _context.UIComponents
			.AsNoTracking();

		foreach (var includeProperty in includeProperties.Split
			(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
		{
			query = query.Include(includeProperty);
		}

		return await query.FirstOrDefaultAsync(x => x.Id == id);
	}

	public async Task<UIComponent> AddAsync(UIComponent entity)
	{
		await _context.UIComponents.AddAsync(entity);
		await _context.SaveChangesAsync();
		return _mapper.Map<UIComponent>(entity);
	}

	public async Task UpdateAsync(UIComponent entity)
	{
		entity.UpdatedAt = DateTime.UtcNow;
		_context.Entry(entity).State = EntityState.Modified;
		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Guid id)
	{
		var entity = await _context.UIComponents.FindAsync(id);
		if (entity != null)
		{
			entity.IsActive = false; // Soft delete
			await _context.SaveChangesAsync();
		}
	}

	public async Task<(IEnumerable<UIComponent> Items, int TotalCount)> SearchAsync(SearchUIComponentRequest request)
	{
		IQueryable<UIComponent> query = _context.UIComponents
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

		// Get total count before pagination
		var totalCount = await query.CountAsync();

		// Apply pagination and include related data
		var items = await query
			.OrderByDescending(x => x.CreatedAt)
			.Include(x => x.Categories)
			.Include(x => x.Tags)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.ToListAsync();

		return (items, totalCount);
	}

	public async Task<(IEnumerable<UIComponent> Items, int TotalCount)> FilterAsync(FilterUIComponentRequest request)
	{
		IQueryable<UIComponent> query = _context.UIComponents
			.AsNoTracking()
			.Where(x => x.IsActive);

		// Filter by framework
		if (!string.IsNullOrEmpty(request.Framework))
		{
			query = query.Where(x => x.Framework == request.Framework);
		}

		// Filter by type
		if (!string.IsNullOrEmpty(request.Type))
		{
			query = query.Where(x => x.Type == request.Type);
		}

		// Filter by price range
		if (request.MinPrice.HasValue)
		{
			query = query.Where(x => x.Price >= request.MinPrice.Value);
		}
		if (request.MaxPrice.HasValue)
		{
			query = query.Where(x => x.Price <= request.MaxPrice.Value);
		}

		// Filter by categories
		if (request.CategoryIds != null && request.CategoryIds.Any())
		{
			query = query.Where(x => x.Categories.Any(c => request.CategoryIds.Contains(c.Id)));
		}

		// Filter by tags
		if (request.TagIds != null && request.TagIds.Any())
		{
			query = query.Where(x => x.Tags.Any(t => request.TagIds.Contains(t.Id)));
		}

		// Apply sorting
		query = request.SortBy?.ToLower() switch
		{
			"price_asc" => query.OrderBy(x => x.Price),
			"price_desc" => query.OrderByDescending(x => x.Price),
			"created_at_desc" => query.OrderByDescending(x => x.CreatedAt),
			_ => query.OrderByDescending(x => x.CreatedAt) // default sorting
		};

		// Get total count before pagination
		var totalCount = await query.CountAsync();

		// Apply pagination and include related data
		var items = await query
			.Include(x => x.Categories)
			.Include(x => x.Tags)
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.ToListAsync();

		return (items, totalCount);
	}
}