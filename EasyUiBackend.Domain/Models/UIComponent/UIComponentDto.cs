namespace EasyUiBackend.Domain.Models.UIComponent
{
    public class UIComponentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
public string? Html { get; set; }
			public string? Css { get; set; }
			public string? Js { get; set; }
        public decimal Price { get; set; }
        public string PreviewImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Downloads { get; set; }
        public int Views { get; set; }
        public double Rating { get; set; }
        
        // Simplified navigation properties
        public ICollection<string> Categories { get; set; }
        public ICollection<string> Tags { get; set; }
    }

    public class UIComponentListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Html { get; set; }
			public string? Css { get; set; }
			public string? Js { get; set; }
        public decimal Price { get; set; }
        public string PreviewImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Downloads { get; set; }
        public double Rating { get; set; }
    }
} 