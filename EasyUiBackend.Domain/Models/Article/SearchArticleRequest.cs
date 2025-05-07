using System;
using EasyUiBackend.Domain.Models.Common;

namespace EasyUiBackend.Domain.Models.Article
{
    public class SearchArticleRequest : PaginationRequest
    {
        public string Keyword { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid? AuthorId { get; set; }
        public string SortBy { get; set; } = "PublishedAt";
        public bool SortDescending { get; set; } = true;
    }
} 