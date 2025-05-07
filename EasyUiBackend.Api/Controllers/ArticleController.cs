using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Article;
using EasyUiBackend.Api.Extensions;
using AutoMapper;
using EasyUiBackend.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyUiBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _repository;
        private readonly IMapper _mapper;

        public ArticleController(IArticleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/Article
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleResponse>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (articles, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize, "Author");
            
            var response = new PaginatedResponse<ArticleResponse>
            {
                Items = _mapper.Map<List<ArticleResponse>>(articles),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
            
            return Ok(response);
        }

        // GET: api/Article/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ArticleResponse>> GetById(Guid id)
        {
            var article = await _repository.GetByIdAsync(id, "Author");
            
            if (article == null)
                return NotFound();

            // Increment view count
            await _repository.IncrementViewCountAsync(id);
            
            return Ok(_mapper.Map<ArticleResponse>(article));
        }

        // GET: api/Article/latest
        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<ArticleSummaryResponse>>> GetLatest([FromQuery] int count = 5)
        {
            var articles = await _repository.GetLatestArticlesAsync(count, "Author");
            return Ok(_mapper.Map<List<ArticleSummaryResponse>>(articles));
        }

        // GET: api/Article/author/{authorId}
        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<IEnumerable<ArticleSummaryResponse>>> GetByAuthor(Guid authorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var articles = await _repository.GetArticlesByAuthorAsync(authorId, "Author");
            
            // Manual pagination
            var totalCount = articles.Count();
            var pagedArticles = articles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            var response = new PaginatedResponse<ArticleSummaryResponse>
            {
                Items = _mapper.Map<List<ArticleSummaryResponse>>(pagedArticles),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
            
            return Ok(response);
        }

        // POST: api/Article/search
        [HttpPost("search")]
        public async Task<ActionResult<PaginatedResponse<ArticleResponse>>> Search([FromBody] SearchArticleRequest request)
        {
            var (articles, totalCount) = await _repository.SearchAsync(request);
            
            var response = new PaginatedResponse<ArticleResponse>
            {
                Items = _mapper.Map<List<ArticleResponse>>(articles),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
            
            return Ok(response);
        }

        // POST: api/Article
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ArticleResponse>> Create([FromBody] CreateArticleRequest request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null");

            var article = _mapper.Map<Article>(request);
            article.AuthorId = User.GetUserId();
            
            if (!request.PublishedAt.HasValue)
                article.PublishedAt = DateTime.UtcNow;
            
            var createdArticle = await _repository.AddAsync(article);
            
            return CreatedAtAction(nameof(GetById), new { id = createdArticle.Id }, _mapper.Map<ArticleResponse>(createdArticle));
        }

        // PUT: api/Article/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleRequest request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null");

            var article = await _repository.GetByIdAsync(id);
            
            if (article == null)
                return NotFound();
            
            var userId = User.GetUserId();
            
            // Only allow the author or admin to update the article
            if (article.AuthorId != userId && !User.IsInRole("Admin"))
                return Forbid();
            
            _mapper.Map(request, article);
            article.UpdatedBy = userId;
            
            await _repository.UpdateAsync(article);
            
            return NoContent();
        }

        // DELETE: api/Article/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var article = await _repository.GetByIdAsync(id);
            
            if (article == null)
                return NotFound();
            
            var userId = User.GetUserId();
            
            // Only allow the author or admin to delete the article
            if (article.AuthorId != userId && !User.IsInRole("Admin"))
                return Forbid();
            
            await _repository.DeleteAsync(id);
            
            return NoContent();
        }
    }
} 