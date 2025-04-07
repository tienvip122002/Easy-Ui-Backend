namespace EasyUiBackend.Domain.Models.Order;

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = null!; // Processing, Shipping, Delivered, Cancelled
} 