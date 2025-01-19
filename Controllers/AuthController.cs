using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using OMbackend.Data;
using OMbackend.Models;

namespace OMbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                return BadRequest("Username already exists");
            }

            var user = new User
            {
                Username = userDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                SecretQuestion = userDto.SecretQuestion,
                SecretAnswer = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                // Check if the user exists in the database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

                if (user == null || string.IsNullOrEmpty(user.Password) || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    return Unauthorized("Invalid username or password");
                }

                // 1. Generate user claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()), // User ID claim
                    new Claim(ClaimTypes.Name, user.Username),                // Username claim
                    new Claim("username", user.Username)                      // Custom claim if needed
                };

                // 2. Create the claims identity with the specified authentication scheme
                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                // 3. Create the authentication properties
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                };

                // 4. Sign in the user and issue the cookie with ClaimsPrincipal
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                // Log successful login for debugging
                Console.WriteLine($"User {user.Username} logged in successfully.");

                return Ok(new { Message = "Verified" });
            }
            catch (Exception ex)
            {
                // Log the error for troubleshooting
                Console.WriteLine($"Error during login: {ex.Message}");

                // Return a more detailed error response if in development environment
                return StatusCode(500, $"An error occurred during login: {ex.Message}");
            }
        }


        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim("username", user.Username)
            };

            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                         ?? throw new InvalidOperationException("JWT_SECRET_KEY environment variable is not set.");
            var jwtIssuer = _configuration["JwtSettings:Issuer"]
                            ?? throw new InvalidOperationException("JwtSettings:Issuer is not configured");
            var jwtAudience = _configuration["JwtSettings:Audience"]
                              ?? throw new InvalidOperationException("JwtSettings:Audience is not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet]
        public IActionResult GetCookie()
        {
            if (Request.Cookies.TryGetValue("AuthToken", out string? cookieValue))
            {
                return Ok(new { Message = "Cookie retrieved successfully!", CookieValue = cookieValue });
            }
            else
            {
                return NotFound(new { Message = "Cookie not found!" });
            }
        }
    }
}