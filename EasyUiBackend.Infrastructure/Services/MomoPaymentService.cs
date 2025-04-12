using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.Payment;
using System.Net.Http.Json;
using System.Text.Json;
using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Infrastructure.Services;

public class MomoPaymentService : IPaymentService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;

    public MomoPaymentService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<string> CreateMomoPaymentAsync(CreateMomoPaymentRequest request)
    {
        try 
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(request.OrderId);
            if (order == null)
            {
                throw new Exception($"Order not found with ID: {request.OrderId}");
            }

            // Convert USD to VND
            var exchangeRate = 24500;
            var amountInVND = (long)Math.Ceiling(order.TotalAmount * exchangeRate);

            // Ensure minimum amount
            if (amountInVND < 1000)
            {
                amountInVND = 1000;
            }

            var requestId = Guid.NewGuid().ToString();
            var orderId = DateTime.UtcNow.Ticks.ToString();

            var momoRequest = new MomoPaymentRequest
            {
                PartnerCode = _configuration["MoMo:PartnerCode"]!,
                RequestId = requestId,
                Amount = amountInVND,
                OrderId = orderId,
                OrderInfo = $"Payment for order {order.Id}",
                RedirectUrl = request.ReturnUrl,
                IpnUrl = "https://localhost:44319/api/payment/momo/ipn",
                RequestType = "captureWallet",
                ExtraData = "",
                Lang = "vi"
            };

            var rawHash = $"accessKey={_configuration["MoMo:AccessKey"]}" +
                         $"&amount={momoRequest.Amount}" +
                         $"&extraData={momoRequest.ExtraData}" +
                         $"&ipnUrl={momoRequest.IpnUrl}" +
                         $"&orderId={momoRequest.OrderId}" +
                         $"&orderInfo={momoRequest.OrderInfo}" +
                         $"&partnerCode={momoRequest.PartnerCode}" +
                         $"&redirectUrl={momoRequest.RedirectUrl}" +
                         $"&requestId={momoRequest.RequestId}" +
                         $"&requestType={momoRequest.RequestType}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_configuration["MoMo:SecretKey"]!));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
            momoRequest.Signature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            using var client = _httpClientFactory.CreateClient();
            
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var requestJson = JsonSerializer.Serialize(momoRequest, jsonSerializerOptions);
            Console.WriteLine($"Request to MoMo: {requestJson}");

            var response = await client.PostAsJsonAsync(_configuration["MoMo:Endpoint"], momoRequest, jsonSerializerOptions);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response from MoMo: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"MoMo API error. Status: {response.StatusCode}, Content: {responseContent}");
            }

            var momoResponse = JsonSerializer.Deserialize<MomoPaymentResponse>(responseContent, jsonSerializerOptions);
            
            if (momoResponse == null)
            {
                throw new Exception("Failed to deserialize MoMo response");
            }

            if (momoResponse.ResultCode != 0) // Changed from "0" to 0
            {
                throw new Exception($"MoMo payment creation failed. Code: {momoResponse.ResultCode}, Message: {momoResponse.Message}");
            }

            // Update order payment info
            order.PaymentProvider = "MoMo";
            order.PaymentStatus = "Pending";
            order.PaymentRequestId = requestId;
            order.PaymentOrderId = orderId;
            await _orderRepository.UpdateAsync(order);

            // Tạo payment record
            var payment = new Payment
            {
                OrderId = order.Id,
                Provider = "MoMo",
                Amount = order.TotalAmount,
                Status = "Pending",
                PaymentUrl = momoResponse.PayUrl,
                ResponseData = responseContent // Lưu response từ MoMo
            };

            await _paymentRepository.AddAsync(payment);

            return momoResponse.PayUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateMomoPaymentAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<bool> ProcessMomoCallbackAsync(IDictionary<string, string> callbackData)
    {
        try 
        {
            if (!IsValidCallback(callbackData))
                return false;

            var orderInfo = callbackData["orderInfo"];
            var orderIdStr = orderInfo.Replace("Payment for order ", "").Trim();
            
            if (!Guid.TryParse(orderIdStr, out Guid orderId))
            {
                Console.WriteLine($"Invalid order ID format: {orderIdStr}");
                return false;
            }

            var order = await _orderRepository.GetByIdAsync(orderId);
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            
            if (order == null || payment == null)
            {
                Console.WriteLine($"Order or Payment not found: {orderId}");
                return false;
            }

            if (callbackData["resultCode"] == "0")
            {
                // Cập nhật Order
                order.PaymentStatus = "Completed";
                order.Status = "Processing";
                order.PaidAt = DateTime.UtcNow;
                order.TransactionId = callbackData["transId"];
                await _orderRepository.UpdateAsync(order);

                // Cập nhật Payment
                payment.Status = "Completed";
                payment.TransactionId = callbackData["transId"];
                payment.PaidAt = DateTime.UtcNow;
                payment.ResponseData = JsonSerializer.Serialize(callbackData);
                await _paymentRepository.UpdateAsync(payment);

                return true;
            }

            // Cập nhật trạng thái thất bại
            order.PaymentStatus = "Failed";
            await _orderRepository.UpdateAsync(order);

            payment.Status = "Failed";
            payment.ResponseData = JsonSerializer.Serialize(callbackData);
            await _paymentRepository.UpdateAsync(payment);

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing MoMo callback: {ex.Message}");
            return false;
        }
    }

    private bool IsValidCallback(IDictionary<string, string> callbackData)
    {
        try
        {
            var rawHash = $"accessKey={_configuration["MoMo:AccessKey"]}" +
                         $"&amount={callbackData["amount"]}" +
                         $"&extraData={callbackData["extraData"]}" +
                         $"&message={callbackData["message"]}" +
                         $"&orderId={callbackData["orderId"]}" +
                         $"&orderInfo={callbackData["orderInfo"]}" +
                         $"&orderType={callbackData["orderType"]}" +
                         $"&partnerCode={callbackData["partnerCode"]}" +
                         $"&payType={callbackData["payType"]}" +
                         $"&requestId={callbackData["requestId"]}" +
                         $"&responseTime={callbackData["responseTime"]}" +
                         $"&resultCode={callbackData["resultCode"]}" +
                         $"&transId={callbackData["transId"]}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_configuration["MoMo:SecretKey"]!));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
            var calculatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return calculatedSignature == callbackData["signature"];
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GetPaymentStatusAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        return order?.PaymentStatus ?? "Unknown";
    }
} 