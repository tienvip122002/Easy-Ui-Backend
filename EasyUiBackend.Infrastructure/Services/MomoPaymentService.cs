using Microsoft.Extensions.Configuration;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.Payment;
using System.Net.Http;
using System.Text.Json;

namespace EasyUiBackend.Infrastructure.Services;

public class MomoPaymentService : IPaymentService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _orderRepository;

    public MomoPaymentService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IOrderRepository orderRepository)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _orderRepository = orderRepository;
    }

    public async Task<string> CreateMomoPaymentAsync(CreateMomoPaymentRequest request)
    {
        var order = await _orderRepository.GetOrderWithItemsAsync(request.OrderId);
        if (order == null)
            throw new Exception("Order not found");

        // Implement Momo payment creation logic here
        // Return payment URL
        return "momo_payment_url";
    }

    public async Task<bool> ProcessMomoCallbackAsync(IDictionary<string, string> callbackData)
    {
        // Implement Momo callback processing logic here
        return true;
    }

    public async Task<string> GetPaymentStatusAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        return order?.PaymentStatus ?? "Unknown";
    }
} 