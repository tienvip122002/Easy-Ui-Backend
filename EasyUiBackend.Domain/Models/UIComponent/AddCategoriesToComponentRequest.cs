namespace EasyUiBackend.Domain.Models.UIComponent
{
    public class AddCategoriesToComponentRequest
    {
        public required List<Guid> CategoryIds { get; set; }
    }
} 