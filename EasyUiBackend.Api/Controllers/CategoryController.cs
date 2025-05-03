using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Category;
using EasyUiBackend.Api.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Infrastructure.Persistence;
using EasyUiBackend.Domain.Models.UIComponent;
using EasyUiBackend.Domain.Models.Common;

namespace EasyUiBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICategoryRepository _repository;

    public CategoryController(AppDbContext context, IMapper mapper, ICategoryRepository repository)
    {
        _context = context;
        _mapper = mapper;
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        var result = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetById(Guid id)
    {
        var result = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create([FromBody] CreateCategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);
        category.CreatedBy = User.GetUserId();

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request)
    {
        var existing = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (existing == null)
            return NotFound();

        _mapper.Map(request, existing);
        existing.UpdatedBy = User.GetUserId();

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
            return NotFound();

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id}/components")]
    public async Task<IActionResult> AddComponents(Guid id, [FromBody] AddComponentsToCategoryRequest request)
    {
        var category = await _context.Categories
            .Include(c => c.Components)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound("Category not found");

        var components = await _context.UIComponents
            .Where(c => request.ComponentIds.Contains(c.Id))
            .ToListAsync();

        if (components.Count != request.ComponentIds.Count)
            return BadRequest("One or more components not found");

        foreach (var component in components)
        {
            if (!category.Components.Any(c => c.Id == component.Id))
            {
                category.Components.Add(component);
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<Category>>> Search([FromBody] SearchCategoryRequest request)
    {
        try
        {
            var results = await _repository.SearchAsync(request);
            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}/components")]
    public async Task<ActionResult<PaginatedResponse<UIComponentListDto>>> GetComponentsByCategoryId(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        // Check if category exists
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (category == null)
            return NotFound("Category not found");
        
        // Get components for this category with pagination
        var query = _context.UIComponents
            .AsNoTracking()
            .Where(c => c.IsActive)
            .Where(c => c.Categories.Any(cat => cat.Id == id))
            .Include(c => c.Categories)
            .Include(c => c.Tags)
            .Include(c => c.Creator);
        
        // Get total count for pagination
        var totalCount = await query.CountAsync();
        
        // Get items with pagination
        var components = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var dtos = _mapper.Map<List<UIComponentListDto>>(components);
        
        // Process each component to set PreviewImage from PreviewUrl if needed
        foreach (var dto in dtos)
        {
            var component = components.FirstOrDefault(c => c.Id == dto.Id);
            if (component != null && string.IsNullOrEmpty(dto.PreviewImage) && !string.IsNullOrEmpty(component.PreviewUrl))
            {
                dto.PreviewImage = component.PreviewUrl;
            }
        }
        
        // Add like information if user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            var userId = User.GetUserId();
            var componentIds = dtos.Select(d => d.Id).ToList();
            
            // Get a repository instance to use existing like functionality
            var uiComponentRepository = HttpContext.RequestServices.GetService<IUIComponentRepository>();
            if (uiComponentRepository != null)
            {
                var likedComponentIds = await uiComponentRepository.GetLikedComponentIdsByUserAsync(userId, componentIds);
                
                foreach (var dto in dtos)
                {
                    dto.IsLikedByCurrentUser = likedComponentIds.Contains(dto.Id);
                }
            }
        }
        
        var response = new PaginatedResponse<UIComponentListDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
        
        return Ok(response);
    }
} 