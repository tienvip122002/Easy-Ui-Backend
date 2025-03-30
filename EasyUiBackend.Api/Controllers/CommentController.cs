using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Models.Comment;
using EasyUiBackend.Api.Extensions;
using AutoMapper;

namespace EasyUiBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ICommentRepository _repository;
    private readonly IMapper _mapper;

    public CommentController(ICommentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("component/{componentId}")]
    public async Task<ActionResult<IEnumerable<Comment>>> GetByComponent(Guid componentId)
    {
        var result = await _repository.GetByComponentIdAsync(componentId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Comment>> GetById(Guid id)
    {
        var result = await _repository.GetByIdAsync(id);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Comment>> Create([FromBody] CreateCommentRequest request)
    {
        var comment = _mapper.Map<Comment>(request);
        comment.CreatedBy = User.GetUserId();

        var result = await _repository.AddAsync(comment);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        // Kiểm tra quyền chỉnh sửa
        if (existing.CreatedBy != User.GetUserId() && !User.IsInRole("Admin"))
            return Forbid();

        existing.Content = request.Content;
        existing.UpdatedBy = User.GetUserId();

        await _repository.UpdateAsync(existing);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var comment = await _repository.GetByIdAsync(id);
        if (comment == null)
            return NotFound();

        // Kiểm tra quyền xóa
        if (comment.CreatedBy != User.GetUserId() && !User.IsInRole("Admin"))
            return Forbid();

        await _repository.DeleteAsync(id);
        return NoContent();
    }
} 