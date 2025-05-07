using System;

namespace EasyUiBackend.Domain.Models.Article
{
    public class CreateArticleRequest
    {
        public required string Title { get; set; }
        public required string ShortDescription { get; set; }
        public required string Description { get; set; }
        public required string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
} 