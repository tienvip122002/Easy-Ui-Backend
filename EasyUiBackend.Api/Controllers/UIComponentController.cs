using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.UIComponent;
using EasyUiBackend.Api.Extensions;
using AutoMapper;

namespace EasyUiBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UIComponentController : ControllerBase
{
	private readonly IUIComponentRepository _repository;
	private readonly IMapper _mapper;

	public UIComponentController(IUIComponentRepository repository, IMapper mapper)
	{
		_repository = repository;
		_mapper = mapper;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<UIComponent>>> GetAll()
	{
		var result = await _repository.GetAllAsync();
		return Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<UIComponent>> GetById(Guid id)
	{
		var result = await _repository.GetByIdAsync(id);
		if (result == null)
			return NotFound();
		return Ok(result);
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
}
