namespace EasyUiBackend.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid UIComponentId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual UIComponent UIComponent { get; set; } = null!;
} 