namespace EasyUiBackend.Domain.Models.Payment;

public class CreateMomoPaymentRequest
{
    public Guid OrderId { get; set; }
    public required string ReturnUrl { get; set; }
} 