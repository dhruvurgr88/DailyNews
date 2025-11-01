using DailyNewsDb.Dtos;
using DailyNewsDb.Modelss;
using DailyNewsServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyNewsWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                EmailId = dto.EmailId,
                Gender = dto.Gender,
                RoleId = dto.RoleId
            };

            var token = await _userService.RegisterUser(user, dto.Password);
            if (token == null) return BadRequest("Email already exists.");

            return Ok(new { token });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _userService.LoginUser(dto.EmailId, dto.Password);
            var role = await _userService.GetRole(dto.EmailId);
            if (token == null) return Unauthorized("Invalid credentials");
            var obj = new { token, role };

            return Ok(obj);
        }
    }
}
