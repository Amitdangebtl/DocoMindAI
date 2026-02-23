using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleUplode.Models;
using SimpleUplode.Services;

namespace SimpleUplode.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest model)
        {
            var result = _authService.Register(model);
            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest model)
        {
            var token = _authService.Login(model);
            return Ok(new { token });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            return Ok(_authService.GetAllUsers());
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("user/{id}")]
        public IActionResult DeleteUser(string id)
        {
            _authService.DeleteUser(id);
            return Ok("User deleted");
        }
 
        [Authorize]
        [HttpGet("me")]
        public IActionResult MyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return Ok(_authService.GetById(userId));
        }
    }
}
