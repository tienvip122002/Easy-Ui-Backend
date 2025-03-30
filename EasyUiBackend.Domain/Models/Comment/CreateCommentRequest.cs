namespace EasyUiBackend.Domain.Models.Comment;

public class CreateCommentRequest
{
    public required string Content { get; set; }
    public required Guid ComponentId { get; set; }
} 