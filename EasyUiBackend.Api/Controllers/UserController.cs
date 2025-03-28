using Application.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EasyUiBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult CreateUser(UserDto userDto)
        {
            _userService.CreateUser(userDto);
            return Ok();
        }
        // ... thêm các phương thức khác nếu cần
    }
} 