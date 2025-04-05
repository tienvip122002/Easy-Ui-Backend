using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Category;
using EasyUiBackend.Api.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Infrastructure.Persistence;

namespace EasyUiBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CategoryController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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
} 