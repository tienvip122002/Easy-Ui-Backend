using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasyUiBackend.Infrastructure.Repositories;

public class UIComponentRepository : IUIComponentRepository
{
	private readonly AppDbContext _context;

	public UIComponentRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<UIComponent>> GetAllAsync()
		=> await _context.UIComponents.ToListAsync();

	public async Task<UIComponent?> GetByIdAsync(Guid id)
		=> await _context.UIComponents.FindAsync(id);

	public async Task<UIComponent> AddAsync(UIComponent entity)
	{
		await _context.UIComponents.AddAsync(entity);
		await _context.SaveChangesAsync();
		return entity;
	}

	public async Task UpdateAsync(UIComponent entity)
	{
		_context.Entry(entity).State = EntityState.Modified;
		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Guid id)
	{
		var entity = await GetByIdAsync(id);
		if (entity != null)
		{
			_context.UIComponents.Remove(entity);
			await _context.SaveChangesAsync();
		}
	}
}
