namespace EasyUiBackend.Domain.Models.Order;

public class PurchasedProductDto
{
    public string OrderId { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }
    public string ProductName { get; set; } = null!;
    public Guid UIComponentId { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentStatus { get; set; } = null!;
} 