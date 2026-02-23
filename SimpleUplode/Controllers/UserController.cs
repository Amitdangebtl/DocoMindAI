using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleUplode.Models;
using SimpleUplode.Services;
using System.Security.Claims;

namespace SimpleUplode.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

   
        [HttpGet("me")]
        public IActionResult MyProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = _userService.GetById(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpPut("me")]
        public IActionResult UpdateProfile([FromBody] UpdateUserRequest model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            _userService.UpdateUser(userId, model);
            return Ok("Profile updated successfully");
        }
    }
}