using EasyUiBackend.Application.Interfaces;
using EasyUiBackend.Domain.Entities;
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

	public async Task<List<UIComponent>> GetAllAsync()
		=> await _context.UIComponents.ToListAsync();

	public async Task<UIComponent?> GetByIdAsync(Guid id)
		=> await _context.UIComponents.FindAsync(id);

	public async Task AddAsync(UIComponent component)
	{
		_context.UIComponents.Add(component);
		await _context.SaveChangesAsync();
	}

	public async Task UpdateAsync(UIComponent component)
	{
		_context.UIComponents.Update(component);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Guid id)
	{
		var item = await _context.UIComponents.FindAsync(id);
		if (item != null)
		{
			_context.UIComponents.Remove(item);
			await _context.SaveChangesAsync();
		}
	}
}
