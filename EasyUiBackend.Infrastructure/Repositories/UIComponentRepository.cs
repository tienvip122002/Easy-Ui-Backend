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