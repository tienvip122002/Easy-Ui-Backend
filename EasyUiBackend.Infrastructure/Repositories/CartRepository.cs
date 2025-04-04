using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Persistence;

namespace EasyUiBackend.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetUserCartAsync(Guid userId)
        {
            return await _context.Carts
                .Include(c => c.UIComponent)
                .Where(c => c.UserId == userId && c.IsActive)
                .ToListAsync();
        }

        public async Task<Cart?> GetByIdAsync(Guid id)
        {
            return await _context.Carts
                .Include(c => c.UIComponent)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<Cart?> GetUserCartItemAsync(Guid userId, Guid componentId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && 
                                        c.UIComponentId == componentId && 
                                        c.IsActive);
        }

        public async Task<Cart> AddAsync(Cart cart)
        {
            cart.CreatedAt = DateTime.UtcNow;
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task UpdateAsync(Cart cart)
        {
            cart.UpdatedAt = DateTime.UtcNow;
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                cart.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }
} 