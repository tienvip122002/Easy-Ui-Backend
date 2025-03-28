using EasyUiBackend.Domain.Entities;
using EasyUiBackend.Domain.Interfaces;
using EasyUiBackend.Infrastructure.Data;

namespace EasyUiBackend.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User GetUserById(int id)
        {
            return _context.Users.Find(id);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        // ... thêm các phương thức khác nếu cần
    }
} 