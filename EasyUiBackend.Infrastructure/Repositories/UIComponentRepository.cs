using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;

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
}