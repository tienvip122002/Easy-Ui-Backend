using System;
using EasyUiBackend.Domain.Models.User;

namespace EasyUiBackend.Domain.Models.Article
{
    public class ArticleResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        
        // Author information
        public Guid? AuthorId { get; set; }
        public UserSummaryResponse Author { get; set; }
        
        // Additional metadata
        public int ViewCount { get; set; }
    }
    
    public class ArticleSummaryResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; }
        public Guid? AuthorId { get; set; }
        public string AuthorName { get; set; }
    }
} 