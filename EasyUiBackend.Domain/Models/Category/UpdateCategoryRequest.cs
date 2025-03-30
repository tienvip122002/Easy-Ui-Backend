namespace EasyUiBackend.Domain.Models.Category;

public class UpdateCategoryRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
} 