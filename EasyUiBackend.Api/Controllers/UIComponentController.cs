using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.UIComponent;
using EasyUiBackend.Domain.Models.Common;
using EasyUiBackend.Api.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Infrastructure.Persistence;
using EasyUiBackend.Api.Models;
namespace EasyUiBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UIComponentController : ControllerBase
{
	private readonly IUIComponentRepository _repository;
	private readonly IMapper _mapper;
	private readonly AppDbContext _context;

	public UIComponentController(IUIComponentRepository repository, IMapper mapper, AppDbContext context)
	{
		_repository = repository;
		_mapper = mapper;
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<UIComponentListDto>>> GetAll()
	{
		var components = await _repository.GetAllAsync(includeProperties: "Categories,Tags");
		var dtos = _mapper.Map<IEnumerable<UIComponentListDto>>(components);
		return Ok(dtos);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<UIComponentDto>> GetById(Guid id)
	{
		var component = await _repository.GetByIdAsync(
			id,
			includeProperties: "Categories,Tags,Comments,Comments.Creator"
		);

		if (component == null)
			return NotFound();

		var dto = _mapper.Map<UIComponentDto>(component);
		return Ok(dto);
	}

	[HttpPost]
	public async Task<ActionResult<UIComponent>> Create([FromBody] CreateUIComponentRequest request)
	{
		var component = _mapper.Map<UIComponent>(request);
		component.CreatedBy = User.GetUserId();

		var result = await _repository.AddAsync(component);
		return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUIComponentRequest request)
	{
		var existing = await _repository.GetByIdAsync(id);
		if (existing == null)
			return NotFound();

		_mapper.Map(request, existing);
		existing.UpdatedBy = User.GetUserId();

		await _repository.UpdateAsync(existing);
		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		await _repository.DeleteAsync(id);
		return NoContent();
	}

	[HttpPost("{id}/categories")]
	public async Task<IActionResult> AddCategories(Guid id, [FromBody] AddCategoriesToComponentRequest request)
	{
		var component = await _context.UIComponents
			.Include(c => c.Categories)
			.FirstOrDefaultAsync(c => c.Id == id);

		if (component == null)
			return NotFound("UI Component not found");

		var categories = await _context.Categories
			.Where(c => request.CategoryIds.Contains(c.Id))
			.ToListAsync();

		if (categories.Count != request.CategoryIds.Count)
			return BadRequest("One or more categories not found");

		foreach (var category in categories)
		{
			if (!component.Categories.Any(c => c.Id == category.Id))
			{
				component.Categories.Add(category);
			}
		}

		await _context.SaveChangesAsync();
		return NoContent();
	}

	[HttpPost("search")]
	public async Task<ActionResult<PaginatedResponse<UIComponentListDto>>> Search([FromBody] SearchUIComponentRequest request)
	{
		try
		{
			var (items, totalCount) = await _repository.SearchAsync(request);
			
			var dtos = _mapper.Map<IEnumerable<UIComponentListDto>>(items);
			
			var response = new PaginatedResponse<UIComponentListDto>
			{
				Items = dtos,
				TotalCount = totalCount,
				PageNumber = request.PageNumber,
				PageSize = request.PageSize,
				TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
			};

			return Ok(response);
		}
		catch (Exception ex)
		{
			return StatusCode(500, "An error occurred while processing your request.");
		}
	}

	[HttpPost("filter")]
	public async Task<ActionResult<PaginatedResponse<UIComponentListDto>>> Filter([FromBody] FilterUIComponentRequest request)
	{
		try
		{
			var (items, totalCount) = await _repository.FilterAsync(request);
			
			var dtos = _mapper.Map<IEnumerable<UIComponentListDto>>(items);
			
			var response = new PaginatedResponse<UIComponentListDto>
			{
				Items = dtos,
				TotalCount = totalCount,
				PageNumber = request.PageNumber,
				PageSize = request.PageSize,
				TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
			};

			return Ok(response);
		}
		catch (Exception ex)
		{
			return StatusCode(500, "An error occurred while processing your request.");
		}
	}
}
