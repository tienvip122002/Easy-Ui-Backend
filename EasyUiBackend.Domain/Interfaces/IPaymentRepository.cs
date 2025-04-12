using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment> AddAsync(Payment payment);
    Task<Payment> UpdateAsync(Payment payment);
    Task<Payment?> GetByOrderIdAsync(Guid orderId);
} 