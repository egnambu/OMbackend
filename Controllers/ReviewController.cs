using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMbackend.Data;
using OMbackend.Models;

namespace OMbackend.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        // Create a new review or reply
        [HttpPost("create")]
        public async Task<ActionResult<Review>> CreateReview([FromBody] Review review)
        {
            if (review == null)
            {
                return BadRequest("Review data is required.");
            }

            try
            {
                // Verify the associated shop exists (only for top-level reviews)
                if (review.ParentReviewID == null)
                {
                    var shopExists = await _context.Shops.AnyAsync(s => s.ID == review.ShopID);
                    if (!shopExists)
                    {
                        return BadRequest($"Shop with ID {review.ShopID} does not exist.");
                    }
                }
                else
                {
                    // Verify the parent review exists (only for replies)
                    var parentExists = await _context.Reviews.AnyAsync(r => r.ID == review.ParentReviewID);
                    if (!parentExists)
                    {
                        return BadRequest($"Parent review with ID {review.ParentReviewID} does not exist.");
                    }
                }

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetReview), new { id = review.ID }, review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the review: {ex.Message}");
            }
        }

        // Get a single review by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Reviews) // Include nested replies
                .FirstOrDefaultAsync(r => r.ID == id);

            if (review == null)
            {
                return NotFound($"Review with ID {id} not found.");
            }

            var reviewDto = MapToDto(review);
            return Ok(reviewDto);
        }

        // Get all reviews and replies for a specific shop
        [HttpGet("shop/{shopId}")]
        public async Task<ActionResult<List<ReviewDto>>> GetReviewsByShopId(int shopId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ShopID == shopId && r.ParentReviewID == null) // Only top-level reviews
                .Include(r => r.Reviews) // Include nested replies
                .ThenInclude(reply => reply.Reviews) // Include deeper levels of replies
                .ToListAsync();

            if (!reviews.Any())
            {
                return NotFound($"No reviews found for Shop ID {shopId}");
            }

            var reviewDtos = reviews.Select(MapToDto).ToList();
            return Ok(reviewDtos);
        }

        // Helper method to map a Review to a ReviewDto
        private ReviewDto MapToDto(Review review)
        {
            return new ReviewDto
            {
                ID = review.ID,
                Content = review.Content,
                Username = review.Username,
                Service = review.Service,
                Stars = review.Stars,
                CreatedAt = review.CreatedAt,
                Reviews = review.Reviews?.Select(MapToDto).ToList() // Map child reviews recursively
            };
        }
    }
}
