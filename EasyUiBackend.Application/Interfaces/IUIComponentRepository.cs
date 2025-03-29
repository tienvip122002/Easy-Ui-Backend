using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Application.Interfaces;

public interface IUIComponentRepository
{
	Task<List<UIComponent>> GetAllAsync();
	Task<UIComponent?> GetByIdAsync(Guid id);
	Task AddAsync(UIComponent component);
	Task UpdateAsync(UIComponent component);
	Task DeleteAsync(Guid id);
}
