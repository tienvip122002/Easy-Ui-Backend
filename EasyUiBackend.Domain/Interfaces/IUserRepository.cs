using EasyUiBackend.Domain.Entities;

namespace EasyUiBackend.Domain.Interfaces
{
    public interface IUserRepository
    {
        User GetUserById(int id);
        void AddUser(User user);
        // ... thêm các phương thức khác nếu cần
    }
} 