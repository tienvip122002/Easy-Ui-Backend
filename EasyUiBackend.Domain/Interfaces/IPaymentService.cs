using EasyUiBackend.Domain.Models.Payment;

namespace EasyUiBackend.Domain.Interfaces;

public interface IPaymentService
{
    Task<string> CreateMomoPaymentAsync(CreateMomoPaymentRequest request);
    Task<bool> ProcessMomoCallbackAsync(IDictionary<string, string> callbackData);
    Task<string> GetPaymentStatusAsync(Guid orderId);
} 