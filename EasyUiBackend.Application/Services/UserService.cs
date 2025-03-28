using EasyUiBackend.Application.Interfaces;
using EasyUiBackend.Application.DTOs;
using EasyUiBackend.Domain.Entities; // Giả sử bạn có một namespace cho Entities
using EasyUiBackend.Domain.Interfaces; // Giả sử bạn có một namespace cho Interfaces

namespace EasyUiBackend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void CreateUser(UserDto userDto)
        {
            var user = new User { Name = userDto.Name, Email = userDto.Email };
            _userRepository.AddUser(user);
        }
        // ... thêm các phương thức khác nếu cần
    }
} 