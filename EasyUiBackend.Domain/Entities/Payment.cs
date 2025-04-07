namespace EasyUiBackend.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; set; }
    public string Provider { get; set; } = null!; // Momo, VNPay, etc.
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public string? TransactionId { get; set; }
    public string? PaymentUrl { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? ResponseData { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
} 