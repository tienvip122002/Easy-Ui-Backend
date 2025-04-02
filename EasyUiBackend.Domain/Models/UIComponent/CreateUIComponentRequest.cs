namespace EasyUiBackend.Domain.Models.UIComponent
{
    public class CreateUIComponentRequest
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? Html { get; set; }
        public string? Css { get; set; }
        public string? Js { get; set; }
        public string? PreviewUrl { get; set; }
        public string? Type { get; set; }
        public string? Framework { get; set; }
    }
} 