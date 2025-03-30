using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Persistence;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace EasyUiBackend.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TagRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags
                .Where(t => t.IsActive)
                .ProjectTo<Tag>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(Guid id)
        {
            return await _context.Tags
                .Where(t => t.Id == id && t.IsActive)
                .ProjectTo<Tag>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<Tag> AddAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return _mapper.Map<Tag>(tag);
        }

        public async Task UpdateAsync(Tag tag)
        {
            tag.UpdatedAt = DateTime.UtcNow;
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                tag.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }
} 