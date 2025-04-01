using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Tag;
using EasyUiBackend.Api.Extensions;
using AutoMapper;

namespace EasyUiBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]  // Comment tạm dòng này để test
public class TagController : ControllerBase
{
    private readonly ITagRepository _repository;
    private readonly IMapper _mapper;

    public TagController(ITagRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
    {
        try
        {
            var tags = await _repository.GetAllAsync();
            var tagDtos = tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy
            });
            return Ok(tagDtos);
        }
        catch (Exception ex)
        {
            // Log error here
            return StatusCode(500, "An error occurred while fetching tags");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TagDto>> GetById(Guid id)
    {
        var tag = await _repository.GetByIdAsync(id);
        if (tag == null)
            return NotFound();
        
        var tagDto = new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            CreatedAt = tag.CreatedAt,
            CreatedBy = tag.CreatedBy
        };
        return Ok(tagDto);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagRequest request)
    {
        var tag = _mapper.Map<Tag>(request);
        tag.CreatedBy = User.GetUserId();

        var result = await _repository.AddAsync(tag);
        var tagDto = new TagDto
        {
            Id = result.Id,
            Name = result.Name,
            CreatedAt = result.CreatedAt,
            CreatedBy = result.CreatedBy
        };
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, tagDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        existing.Name = request.Name;
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