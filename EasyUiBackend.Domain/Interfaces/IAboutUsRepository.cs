using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.AboutUs;

namespace EasyUiBackend.Domain.Interfaces;

public interface IAboutUsRepository
{
    Task<IEnumerable<AboutUs>> GetAllAsync();
    Task<AboutUs?> GetByIdAsync(Guid id);
    Task<AboutUs> AddAsync(AboutUs aboutUs);
    Task UpdateAsync(AboutUs aboutUs);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<AboutUs>> SearchAsync(SearchAboutUsRequest request);
} 