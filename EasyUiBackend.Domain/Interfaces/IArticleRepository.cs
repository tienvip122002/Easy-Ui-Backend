using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Article;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetAllAsync(string includeProperties = "");
        Task<(IEnumerable<Article> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string includeProperties = "");
        Task<Article> GetByIdAsync(Guid id, string includeProperties = "");
        Task<Article> AddAsync(Article entity);
        Task UpdateAsync(Article entity);
        Task DeleteAsync(Guid id);
        Task<(IEnumerable<Article> Items, int TotalCount)> SearchAsync(SearchArticleRequest request);
        Task<IEnumerable<Article>> GetLatestArticlesAsync(int count, string includeProperties = "");
        Task<IEnumerable<Article>> GetArticlesByAuthorAsync(Guid authorId, string includeProperties = "");
        Task IncrementViewCountAsync(Guid articleId);
    }
} 