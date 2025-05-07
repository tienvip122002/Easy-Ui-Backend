using System;
using System.Collections.Generic;

namespace EasyUiBackend.Domain.Entities
{
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime PublishedAt { get; set; }
        
        // Author reference
        public Guid? AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }
        
        // Image URL for the article
        public string ImageUrl { get; set; }
        
        // Tracking metrics
        public int ViewCount { get; set; } = 0;
    }
} 