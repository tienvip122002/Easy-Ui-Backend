using EasyUiBackend.Domain.Models.Comment;

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
        public int LikesCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        
        // Simplified navigation properties
        public ICollection<string> Categories { get; set; }
        public ICollection<string> Tags { get; set; }
        
        // Comments collection
        public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
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
        public int LikesCount { get; set; }
        public int Views { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        
        // Thông tin tác giả
        public CreatorDto? Creator { get; set; }
    }
    
    public class CreatorDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
    }
} 