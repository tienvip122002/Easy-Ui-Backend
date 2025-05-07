using System;

namespace EasyUiBackend.Domain.Models.Article
{
    public class ViewCount
    {
        public Guid ArticleId { get; set; }
        public int Count { get; set; }
    }
} 