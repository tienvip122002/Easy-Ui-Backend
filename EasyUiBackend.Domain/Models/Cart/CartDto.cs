namespace EasyUiBackend.Domain.Models.Cart
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public Guid UIComponentId { get; set; }
        public string UIComponentName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCartRequest
    {
        
        public Guid UIComponentId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest
    {
        public int Quantity { get; set; }
    }
} 