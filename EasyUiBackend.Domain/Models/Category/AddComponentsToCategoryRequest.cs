namespace EasyUiBackend.Domain.Models.Category
{
    public class AddComponentsToCategoryRequest
    {
        public required List<Guid> ComponentIds { get; set; }
    }
} 