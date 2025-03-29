using EasyUiBackend.Application.Interfaces;
using EasyUiBackend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EasyUiBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UIComponentController : ControllerBase
{
	private readonly IUIComponentRepository _repo;

	public UIComponentController(IUIComponentRepository repo)
	{
		_repo = repo;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll() => Ok(await _repo.GetAllAsync());

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(Guid id)
	{
		var result = await _repo.GetByIdAsync(id);
		return result is null ? NotFound() : Ok(result);
	}

	[HttpPost]
	public async Task<IActionResult> Create(UIComponent input)
	{
		input.Id = Guid.NewGuid();
		await _repo.AddAsync(input);
		return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(Guid id, UIComponent input)
	{
		var existing = await _repo.GetByIdAsync(id);
		if (existing is null) return NotFound();

		existing.Name = input.Name;
		await _repo.UpdateAsync(existing);
		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(Guid id)
	{
		await _repo.DeleteAsync(id);
		return NoContent();
	}
}
