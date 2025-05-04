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
		// Tự động include Creator để lấy thông tin tác giả
		string effectiveIncludeProperties = includeProperties;
		if (!includeProperties.Contains("Creator"))
		{
			effectiveIncludeProperties = string.IsNullOrEmpty(includeProperties) ? 
				"Creator" : 
				includeProperties + ",Creator";
		}
		
		IQueryable<UIComponent> query = _context.UIComponents
			.AsNoTracking()
			.Where(x => x.IsActive); // Chỉ lấy các component đang active

		foreach (var includeProperty in effectiveIncludeProperties.Split
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
			"likes_desc" => query.OrderByDescending(x => x.LikesCount),
			"views_desc" => query.OrderByDescending(x => x.Views),
			"popular" => query.OrderByDescending(x => (x.Views * 0.7) + (x.LikesCount * 0.3)), // Kết hợp views và likes
			_ => query.OrderByDescending(x => x.CreatedAt) // default sorting
		};

		// Get total count before pagination
		var totalCount = await query.CountAsync();

		// Apply pagination and include related data
		var items = await query
			.Include(x => x.Categories)
			.Include(x => x.Tags)
			.Include(x => x.Creator) // Thêm Creator để trả về thông tin tác giả
			.Skip((request.PageNumber - 1) * request.PageSize)
			.Take(request.PageSize)
			.ToListAsync();

		return (items, totalCount);
	}

	// Like functionality implementation
	public async Task<bool> LikeComponentAsync(Guid componentId, Guid userId)
	{
		// Check if component exists
		var component = await _context.UIComponents.FindAsync(componentId);
		if (component == null)
			return false;

		// Check if user already liked this component
		var existingLike = await _context.ComponentLikes
			.FirstOrDefaultAsync(cl => cl.UIComponentId == componentId && cl.UserId == userId);

		if (existingLike != null)
			return true; // Already liked

		// Create new like
		var like = new ComponentLike
		{
			UIComponentId = componentId,
			UserId = userId,
			LikedAt = DateTime.UtcNow
		};

		_context.ComponentLikes.Add(like);

		// Increment like count on component
		component.LikesCount += 1;

		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> UnlikeComponentAsync(Guid componentId, Guid userId)
	{
		// Find the like
		var like = await _context.ComponentLikes
			.FirstOrDefaultAsync(cl => cl.UIComponentId == componentId && cl.UserId == userId);

		if (like == null)
			return false; // Not liked previously

		// Remove the like
		_context.ComponentLikes.Remove(like);

		// Decrement like count on component
		var component = await _context.UIComponents.FindAsync(componentId);
		if (component != null && component.LikesCount > 0)
		{
			component.LikesCount -= 1;
		}

		await _context.SaveChangesAsync();
		return true;
	}

	public async Task<bool> IsLikedByUserAsync(Guid componentId, Guid userId)
	{
		return await _context.ComponentLikes
			.AnyAsync(cl => cl.UIComponentId == componentId && cl.UserId == userId);
	}

	public async Task<IEnumerable<ComponentLike>> GetComponentLikesAsync(Guid componentId)
	{
		return await _context.ComponentLikes
			.Where(cl => cl.UIComponentId == componentId)
			.Include(cl => cl.User)
			.OrderByDescending(cl => cl.LikedAt)
			.ToListAsync();
	}

	public async Task<IEnumerable<UIComponent>> GetUserLikedComponentsAsync(Guid userId, string includeProperties = "")
	{
		// Lấy danh sách ComponentId mà user đã like
		var likedComponentIds = await _context.ComponentLikes
			.Where(cl => cl.UserId == userId)
			.Select(cl => cl.UIComponentId)
			.ToListAsync();

		// Sau đó query các components dựa trên danh sách ID đó
		IQueryable<UIComponent> query = _context.UIComponents
			.AsNoTracking()
			.Where(x => x.IsActive && likedComponentIds.Contains(x.Id));

		// Áp dụng include properties
		foreach (var includeProperty in includeProperties.Split
			(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
		{
			query = query.Include(includeProperty);
		}

		return await query
			.OrderByDescending(x => x.CreatedAt)
			.ToListAsync();
	}

	// View tracking implementation
	public async Task IncrementViewCountAsync(Guid componentId)
	{
		var component = await _context.UIComponents.FindAsync(componentId);
		if (component != null)
		{
			component.Views += 1;
			await _context.SaveChangesAsync();
		}
	}

	public async Task<(IEnumerable<UIComponent> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string includeProperties = "")
	{
		// Automatically include Creator to get author information
		string effectiveIncludeProperties = includeProperties;
		if (!includeProperties.Contains("Creator"))
		{
			effectiveIncludeProperties = string.IsNullOrEmpty(includeProperties) ? 
				"Creator" : 
				includeProperties + ",Creator";
		}
		
		IQueryable<UIComponent> query = _context.UIComponents
			.AsNoTracking()
			.Where(x => x.IsActive);

		foreach (var includeProperty in effectiveIncludeProperties.Split
			(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
		{
			query = query.Include(includeProperty);
		}

		var totalCount = await query.CountAsync();

		var items = await query
			.OrderByDescending(x => x.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		return (items, totalCount);
	}

	public async Task<ICollection<Guid>> GetLikedComponentIdsByUserAsync(Guid userId, ICollection<Guid> componentIds)
	{
		return await _context.ComponentLikes
			.Where(cl => cl.UserId == userId && componentIds.Contains(cl.UIComponentId))
			.Select(cl => cl.UIComponentId)
			.ToListAsync();
	}
}