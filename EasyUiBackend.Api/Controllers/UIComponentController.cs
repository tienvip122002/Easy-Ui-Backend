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
		
		// Check if the current user has liked these components
		if (User.Identity.IsAuthenticated)
		{
			var userId = User.GetUserId();
			foreach (var dto in dtos)
			{
				dto.IsLikedByCurrentUser = await _repository.IsLikedByUserAsync(dto.Id, userId);
			}
		}
		
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

		// Tăng số lượng views khi API GetById được gọi
		await _repository.IncrementViewCountAsync(id);

		var dto = _mapper.Map<UIComponentDto>(component);
		
		// Gán giá trị PreviewUrl vào PreviewImage nếu PreviewImage đang null
		if (string.IsNullOrEmpty(dto.PreviewImage) && !string.IsNullOrEmpty(component.PreviewUrl))
		{
			dto.PreviewImage = component.PreviewUrl;
		}
		
		// Check if the current user has liked this component
		if (User.Identity.IsAuthenticated)
		{
			var userId = User.GetUserId();
			dto.IsLikedByCurrentUser = await _repository.IsLikedByUserAsync(id, userId);
		}
		
		return Ok(dto);
	}

	[HttpPost]
	public async Task<ActionResult<UIComponent>> Create([FromBody] CreateUIComponentRequest request)
	{
		// Đảm bảo Price có giá trị mặc định là 0 nếu không được cung cấp
		if (request.Price == default)
		{
			request.Price = 0;
		}

		// Đảm bảo DiscountPrice có giá trị mặc định là 0 nếu không được cung cấp
		if (request.DiscountPrice == null)
		{
			request.DiscountPrice = 0;
		}

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

		// Đảm bảo Price có giá trị mặc định là 0 nếu không được cung cấp
		if (request.Price == default)
		{
			request.Price = 0;
		}

		// Đảm bảo DiscountPrice có giá trị mặc định là 0 nếu không được cung cấp
		if (request.DiscountPrice == null)
		{
			request.DiscountPrice = 0;
		}

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
	
	[HttpPost("{id}/tags")]
	public async Task<IActionResult> AddTags(Guid id, [FromBody] AddTagsToComponentRequest request)
	{
		var component = await _context.UIComponents
			.Include(c => c.Tags)
			.FirstOrDefaultAsync(c => c.Id == id);

		if (component == null)
			return NotFound("UI Component not found");

		var tags = await _context.Tags
			.Where(t => request.TagIds.Contains(t.Id))
			.ToListAsync();

		if (tags.Count != request.TagIds.Count)
			return BadRequest("One or more tags not found");

		foreach (var tag in tags)
		{
			if (!component.Tags.Any(t => t.Id == tag.Id))
			{
				component.Tags.Add(tag);
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
			
			// Check if the current user has liked these components
			if (User.Identity.IsAuthenticated)
			{
				var userId = User.GetUserId();
				foreach (var dto in dtos)
				{
					dto.IsLikedByCurrentUser = await _repository.IsLikedByUserAsync(dto.Id, userId);
				}
			}
			
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
			
			// Check if the current user has liked these components
			if (User.Identity.IsAuthenticated)
			{
				var userId = User.GetUserId();
				foreach (var dto in dtos)
				{
					dto.IsLikedByCurrentUser = await _repository.IsLikedByUserAsync(dto.Id, userId);
				}
			}
			
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

	// Like functionality endpoints
	[HttpPost("{id}/like")]
	public async Task<IActionResult> LikeComponent(Guid id)
	{
		if (!User.Identity.IsAuthenticated)
			return Unauthorized();

		var userId = User.GetUserId();
		var result = await _repository.LikeComponentAsync(id, userId);

		if (!result)
			return NotFound("Component not found");

		return NoContent();
	}

	[HttpPost("{id}/unlike")]
	public async Task<IActionResult> UnlikeComponent(Guid id)
	{
		if (!User.Identity.IsAuthenticated)
			return Unauthorized();

		var userId = User.GetUserId();
		var result = await _repository.UnlikeComponentAsync(id, userId);

		if (!result)
			return NotFound("Component or like not found");

		return NoContent();
	}

	[HttpGet("{id}/likes")]
	public async Task<ActionResult<IEnumerable<ComponentLikeDto>>> GetComponentLikes(Guid id)
	{
		var component = await _repository.GetByIdAsync(id);
		if (component == null)
			return NotFound("Component not found");

		var likes = await _repository.GetComponentLikesAsync(id);
		var dtos = _mapper.Map<IEnumerable<ComponentLikeDto>>(likes);

		return Ok(dtos);
	}

	[HttpGet("liked")]
	public async Task<ActionResult<IEnumerable<UIComponentListDto>>> GetLikedComponents()
	{
		if (!User.Identity.IsAuthenticated)
			return Unauthorized();

		var userId = User.GetUserId();
		var components = await _repository.GetUserLikedComponentsAsync(userId, "Categories,Tags");
		var dtos = _mapper.Map<IEnumerable<UIComponentListDto>>(components);

		// Set isLikedByCurrentUser to true for all components in this list
		foreach (var dto in dtos)
		{
			dto.IsLikedByCurrentUser = true;
		}

		return Ok(dtos);
	}
}
