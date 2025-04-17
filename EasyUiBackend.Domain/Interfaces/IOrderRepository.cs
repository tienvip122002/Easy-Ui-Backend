using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Domain.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetUserOrdersAsync(Guid userId);
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task<Order?> GetOrderWithItemsAsync(Guid id);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task UpdateOrderStatusAsync(Guid id, string status);
    Task<IEnumerable<Order>> GetPurchasedProductsAsync(Guid userId);
} 