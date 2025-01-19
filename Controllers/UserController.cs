using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMbackend.Data;

namespace OMbackend.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth", Policy = "UserPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;


        [HttpGet("userinfo")]
        public IActionResult GetUserInfo()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst("username")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username))
            {
                return Unauthorized("User information could not be retrieved.");
            }
            return Ok(new { UserId = userId, Username = username });
        }
    }
}