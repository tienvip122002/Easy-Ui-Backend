namespace EasyUiBackend.Domain.Models.AboutUs;

public class UpdateAboutUsRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? ImageUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
} 