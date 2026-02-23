using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleUplode.Services;

namespace SimpleUplode.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            return Ok(_adminService.GetAllUsers());
        }


        [HttpGet("user/{id}")]
        public IActionResult GetUserById(string id)
        {
            var user = _adminService.GetUserById(id);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }


        [HttpDelete("user/{id}")]
        public IActionResult DeleteUser(string id)
        {
            _adminService.DeleteUser(id);
            return Ok("User deleted successfully");
        }

        [HttpGet("documents")]
        public IActionResult GetAllDocuments()
        {
            return Ok(_adminService.GetAllDocuments());
        }
    }
}