using Application.DTOs;

namespace EasyUiBackend.Application.Interfaces
{
    public interface IUserService
    {
        void CreateUser(UserDto userDto);
        // ... thêm các phương thức khác nếu cần
    }
} 