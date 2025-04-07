namespace EasyUiBackend.Domain.Models.Payment;

public class CreateMomoPaymentRequest
{
    public Guid OrderId { get; set; }
    public string ReturnUrl { get; set; } = null!;
} 