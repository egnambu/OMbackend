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
    public class ShopController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ShopController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET endpoints
        [HttpGet("ShopByServices/{service}")]
        public async Task<ActionResult<List<object>>> GetShopTypes(string service)
        {
            var shopResult = await _context.Shops
                .Where(sh => sh.Services == service)
                .Select(sh => new
                {
                    sh.ID,
                    sh.Services,
                    sh.Name,
                    sh.Heading,
                    sh.Description,
                    sh.Reviews,
                    sh.Trust,
                    sh.IconsIndex,
                    sh.Ratings,
                    sh.Multiplier,
                    sh.IsAvailable,
                    sh.PaymentMethods,
                    sh.Color01,
                    sh.Color02,
                    sh.Color03,
                    sh.Image,
                    sh.Users,
                    sh.Contact,
                })
                .ToListAsync();

            if (!shopResult.Any())
            {
                return NotFound($"No Stores found : {service}");
            }

            return Ok(shopResult);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<object>>> GetAllShops()
        {
            var allShops = await _context.Shops
                .Where(sh => sh.ID != 26) // Exclude shop with ID 26
                .Select(sh => new
                {
                    sh.ID,
                    sh.Services,
                    sh.Name,
                    sh.Heading,
                    sh.Description,
                    sh.Reviews,
                    sh.Trust,
                    sh.IconsIndex,
                    sh.Ratings,
                    sh.Multiplier,
                    sh.IsAvailable,
                    sh.PaymentMethods,
                    sh.Color01,
                    sh.Color02,
                    sh.Color03,
                    sh.Image,
                    sh.Users,
                    sh.Contact,
                })
                .ToListAsync();

            if (!allShops.Any())
            {
                return NotFound("No shops found in the database.");
            }

            return Ok(allShops);
        }


        [HttpGet("by-trust")]
        public async Task<ActionResult<List<object>>> GetShopsByHighestTrust()
        {
            var shopsByTrust = await _context.Shops
                .Where(sh => sh.ID != 26)
                .OrderByDescending(sh => sh.Trust)
                .Select(sh => new
                {
                    sh.ID,
                    sh.Services,
                    sh.Name,
                    sh.Heading,
                    sh.Description,
                    sh.Reviews,
                    sh.Trust,
                    sh.IconsIndex,
                    sh.Ratings,
                    sh.Multiplier,
                    sh.IsAvailable,
                    sh.PaymentMethods,
                    sh.Color01,
                    sh.Color02,
                    sh.Color03,
                    sh.Image,
                    sh.Users,
                    sh.Contact,
                })
                .ToListAsync();

            if (!shopsByTrust.Any())
            {
                return NotFound("No shops found in the database.");
            }

            return Ok(shopsByTrust);
        }
        [HttpGet("by-reviews")]
        public async Task<ActionResult<List<object>>> GetShopsByHighestReviews()
        {
            var shopsByReviews = await _context.Shops
                .Where(sh => sh.ID != 26) // Exclude shop with ID 26
                .OrderByDescending(sh => sh.Reviews)
                .Select(sh => new
                {
                    sh.ID,
                    sh.Services,
                    sh.Name,
                    sh.Heading,
                    sh.Description,
                    sh.Reviews,
                    sh.Trust,
                    sh.IconsIndex,
                    sh.Ratings,
                    sh.Multiplier,
                    sh.IsAvailable,
                    sh.PaymentMethods,
                    sh.Color01,
                    sh.Color02,
                    sh.Color03,
                    sh.Image,
                    sh.Users,
                    sh.Contact,
                })
                .ToListAsync();

            if (!shopsByReviews.Any())
            {
                return NotFound("No shops found in the database.");
            }

            return Ok(shopsByReviews);
        }


        [HttpGet("Store/{storeName}")]
        public async Task<ActionResult<List<object>>> GetStorebyName(string storeName)
        {
            var storeResult = await _context.Shops
                .Where(sh => sh.Name == storeName)
                .Select(sh => new
                {
                    sh.ID,
                    sh.Services,
                    sh.Name,
                    sh.Heading,
                    sh.Description,
                    sh.Reviews,
                    sh.Trust,
                    sh.IconsIndex,
                    sh.Ratings,
                    sh.Multiplier,
                    sh.IsAvailable,
                    sh.PaymentMethods,
                    sh.Color01,
                    sh.Color02,
                    sh.Color03,
                    sh.Image,
                    sh.Users,
                    sh.Contact,
                })
                .ToListAsync();

            if (!storeResult.Any())
            {
                return NotFound($"No Stores found : {storeName}");
            }

            return Ok(storeResult);
        }

        [HttpGet("Details/{id}")]
        public async Task<ActionResult<Shop>> GetShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);

            if (shop == null)
            {
                return NotFound();
            }

            return shop;
        }

        // POST endpoint
        [HttpPost]
        public async Task<ActionResult<Shop>> PostShop(Shop shop)
        {
            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetShop), new { id = shop.ID }, shop);
        }

        // DELETE by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
            {
                return NotFound($"Shop with ID {id} not found.");
            }

            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();

            return Ok($"Shop with ID {id} deleted successfully.");
        }

        // UPDATE by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShop(int id, Shop updatedShop)
        {
            if (id != updatedShop.ID)
            {
                return BadRequest("Shop ID in the URL does not match the ID in the body.");
            }

            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
            {
                return NotFound($"Shop with ID {id} not found.");
            }

            // Update shop properties
            shop.Services = updatedShop.Services;
            shop.Name = updatedShop.Name;
            shop.Heading = updatedShop.Heading;
            shop.Description = updatedShop.Description;
            shop.Reviews = updatedShop.Reviews;
            shop.Trust = updatedShop.Trust;
            shop.IconsIndex = updatedShop.IconsIndex;
            shop.Ratings = updatedShop.Ratings;
            shop.Multiplier = updatedShop.Multiplier;
            shop.IsAvailable = updatedShop.IsAvailable;
            shop.PaymentMethods = updatedShop.PaymentMethods;
            shop.Users = updatedShop.Users;
            shop.Color01 = updatedShop.Color01;
            shop.Color02 = updatedShop.Color02;
            shop.Color03 = updatedShop.Color03;
            shop.Image = updatedShop.Image;
            shop.Contact = updatedShop.Contact;

            _context.Entry(shop).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok($"Shop with ID {id} updated successfully.");
        }
    }
}
