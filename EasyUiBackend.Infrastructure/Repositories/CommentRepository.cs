using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Persistence;

namespace EasyUiBackend.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public CommentRepository(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Comment>> GetAllAsync()
    {
        return await _context.Comments
            .Where(c => c.IsActive)
            .ProjectTo<Comment>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Comment?> GetByIdAsync(Guid id)
    {
        return await _context.Comments
            .Where(c => c.Id == id && c.IsActive)
            .ProjectTo<Comment>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Comment>> GetByComponentIdAsync(Guid componentId)
    {
        return await _context.Comments
            .Where(c => c.ComponentId == componentId && c.IsActive)
            .ProjectTo<Comment>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return _mapper.Map<Comment>(comment);
    }

    public async Task UpdateAsync(Comment comment)
    {
        comment.UpdatedAt = DateTime.UtcNow;
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment != null)
        {
            comment.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
} 