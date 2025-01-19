using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OMbackend.Data;
using OMbackend.Models;
using Microsoft.AspNetCore.Authorization;

namespace OMbackend.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth", Policy = "UserPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{services}")]
        public async Task<ActionResult<List<Service>>> GetServices(int services)
        {
            // Map `services` parameter to specific ID groups
            var serviceIds = services switch
            {
                1 => new List<int> { 1, 2, 3, 4, 5, 6, 7 },
                2 => new List<int> { 8, 9, 10, 11, 12, 13, 14 },
                3 => new List<int> { 15, 16, 17, 18, 19, 20, 21 },
                4 => new List<int> { 22, 23, 24, 25, 1, 2, 3 },
                5 => new List<int> { 4, 5, 6, 7, 8, 9, 10 },
                6 => new List<int> { 11, 12, 13, 14, 15, 16, 17 },
                7 => new List<int> { 18, 19, 20, 21, 22, 23, 24 },
                8 => new List<int> { 25, 1, 2, 3, 4, 5, 6 },
                9 => new List<int> { 7, 8, 9, 10, 11, 12, 13 },
                10 => new List<int> { 14, 15, 16, 17, 18, 19, 20 },
                11 => new List<int> { 21, 22, 23, 24, 25, 1, 2 },
                12 => new List<int> { 3, 4, 5, 6, 7, 8, 9 },
                13 => new List<int> { 10, 11, 12, 13, 14, 15, 16 },
                14 => new List<int> { 17, 18, 19, 20, 21, 22, 23 },
                15 => new List<int> { 24, 25, 1, 2, 3, 4, 5 },
                16 => new List<int> { 6, 7, 8, 9, 10, 11, 12 },
                17 => new List<int> { 13, 14, 15, 16, 17, 18, 19 },
                18 => new List<int> { 1, 2, 3, 4, 5},
                19 => new List<int> { 2, 3, 4, 5, 6, 7, 8 },
                20 => new List<int> { 9, 10, 11, 12, 13, 14, 15 },
                21 => new List<int> { 16, 17, 18, 19, 20, 21, 22 },
                22 => new List<int> { 23, 24, 25, 1, 2, 3, 4 },
                23 => new List<int> { 5, 6, 7, 8, 9, 10, 11 },
                24 => new List<int> { 12, 13, 14, 15, 16, 17, 18 },
                25 => new List<int> { 19, 20, 21, 22, 23, 24, 25 },

                _ => null // If services doesn't match any group, return null
            };

            if (serviceIds == null)
            {
                return BadRequest($"Invalid service type: {services}");
            }

            // Query database for matching rows
            var results = await _context.Services
                .Where(s => serviceIds.Contains(s.ID))
                .ToListAsync();

            if (!results.Any())
            {
                return NotFound($"No services found for type {services}");
            }

            return Ok(results);
        }
    }
}