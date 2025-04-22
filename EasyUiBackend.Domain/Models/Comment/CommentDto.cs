namespace EasyUiBackend.Domain.Models.Comment;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public Guid ComponentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    
    // Creator info (could expand with more user details if needed)
    public string? CreatorName { get; set; }
}
