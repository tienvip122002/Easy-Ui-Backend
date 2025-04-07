namespace EasyUiBackend.Domain.Entities;

public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Cancelled
    public string? PaymentMethod { get; set; }
    public string? PaymentStatus { get; set; } // Pending, Completed, Failed
    public string? TransactionId { get; set; }
    public DateTime? PaidAt { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ApplicationUser? Creator { get; set; }
    public virtual ApplicationUser? Updater { get; set; }
    public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
} 