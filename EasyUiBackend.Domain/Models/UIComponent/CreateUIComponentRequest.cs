namespace EasyUiBackend.Domain.Models.UIComponent
{
    public class CreateUIComponentRequest
    {
        public required string Name { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; }
        public string? PreviewUrl { get; set; }
        public string? Type { get; set; }
        public string? Framework { get; set; }
    }
} 