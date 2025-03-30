using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Persistence;
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

	public async Task<IEnumerable<UIComponent>> GetAllAsync()
	{
		return await _context.UIComponents
			.Where(c => c.IsActive)
			.ProjectTo<UIComponent>(_mapper.ConfigurationProvider)
			.ToListAsync();
	}

	public async Task<UIComponent?> GetByIdAsync(Guid id)
	{
		return await _context.UIComponents
			.Where(c => c.Id == id && c.IsActive)
			.ProjectTo<UIComponent>(_mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();
	}

	public async Task<UIComponent> AddAsync(UIComponent component)
	{
		await _context.UIComponents.AddAsync(component);
		await _context.SaveChangesAsync();
		return _mapper.Map<UIComponent>(component);
	}

	public async Task UpdateAsync(UIComponent component)
	{
		component.UpdatedAt = DateTime.UtcNow;
		_context.UIComponents.Update(component);
		await _context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Guid id)
	{
		var component = await _context.UIComponents.FindAsync(id);
		if (component != null)
		{
			component.IsActive = false;
			await _context.SaveChangesAsync();
		}
	}
}
