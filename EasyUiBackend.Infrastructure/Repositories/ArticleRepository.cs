using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Models.Article;
using EasyUiBackend.Infrastructure.Persistence;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyUiBackend.Infrastructure.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ArticleRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Article>> GetAllAsync(string includeProperties = "")
        {
            IQueryable<Article> query = _context.Articles
                .AsNoTracking()
                .Where(a => a.IsActive);

            // Include related properties if specified
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query
                .OrderByDescending(a => a.PublishedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Article> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string includeProperties = "")
        {
            IQueryable<Article> query = _context.Articles
                .AsNoTracking()
                .Where(a => a.IsActive);

            // Include related properties if specified
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.PublishedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Article> GetByIdAsync(Guid id, string includeProperties = "")
        {
            IQueryable<Article> query = _context.Articles
                .Where(a => a.Id == id && a.IsActive);

            // Include related properties if specified
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Article> AddAsync(Article entity)
        {
            await _context.Articles.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Article entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Articles.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                article.IsActive = false;
                article.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<Article> Items, int TotalCount)> SearchAsync(SearchArticleRequest request)
        {
            IQueryable<Article> query = _context.Articles
                .AsNoTracking()
                .Where(a => a.IsActive);

            // Search by keyword in title, short description, and content
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var keyword = request.Keyword.ToLower();
                query = query.Where(a =>
                    EF.Functions.Like(a.Title.ToLower(), $"%{keyword}%") ||
                    EF.Functions.Like(a.ShortDescription.ToLower(), $"%{keyword}%") ||
                    EF.Functions.Like(a.Description.ToLower(), $"%{keyword}%") ||
                    EF.Functions.Like(a.Content.ToLower(), $"%{keyword}%")
                );
            }

            // Filter by author
            if (request.AuthorId.HasValue)
            {
                query = query.Where(a => a.AuthorId == request.AuthorId.Value);
            }

            // Filter by date range
            if (request.FromDate.HasValue)
            {
                query = query.Where(a => a.PublishedAt >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(a => a.PublishedAt <= request.ToDate.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortDescending);

            // Apply pagination
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(a => a.Author)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Article>> GetLatestArticlesAsync(int count, string includeProperties = "")
        {
            IQueryable<Article> query = _context.Articles
                .AsNoTracking()
                .Where(a => a.IsActive);

            // Include related properties if specified
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query
                .OrderByDescending(a => a.PublishedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Article>> GetArticlesByAuthorAsync(Guid authorId, string includeProperties = "")
        {
            IQueryable<Article> query = _context.Articles
                .AsNoTracking()
                .Where(a => a.IsActive && a.AuthorId == authorId);

            // Include related properties if specified
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query
                .OrderByDescending(a => a.PublishedAt)
                .ToListAsync();
        }

        public async Task IncrementViewCountAsync(Guid articleId)
        {
            var article = await _context.Articles.FindAsync(articleId);
            if (article != null)
            {
                article.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        private IQueryable<Article> ApplySorting(IQueryable<Article> query, string sortBy, bool sortDescending)
        {
            switch (sortBy.ToLower())
            {
                case "title":
                    return sortDescending
                        ? query.OrderByDescending(a => a.Title)
                        : query.OrderBy(a => a.Title);
                case "createdat":
                    return sortDescending
                        ? query.OrderByDescending(a => a.CreatedAt)
                        : query.OrderBy(a => a.CreatedAt);
                case "publishedat":
                default:
                    return sortDescending
                        ? query.OrderByDescending(a => a.PublishedAt)
                        : query.OrderBy(a => a.PublishedAt);
            }
        }
    }
} 