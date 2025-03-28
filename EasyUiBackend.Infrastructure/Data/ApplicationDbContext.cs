using Microsoft.EntityFrameworkCore;
using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        // ... thêm các DbSet khác nếu cần
    }
} 