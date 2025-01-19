using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMbackend.Data;
using OMbackend.Models;

namespace OMbackend.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth", Policy = "UserPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        // Create a new comment or reply
        [HttpPost("create")]
        public async Task<ActionResult<Comment>> CreateComment([FromBody] Comment comment)
        {
            if (comment == null)
            {
                return BadRequest("Comment data is required.");
            }

            try
            {
                // Verify the associated post exists (only for top-level comments)
                if (comment.ParentCommentID == null)
                {
                    var postExists = await _context.Posts.AnyAsync(p => p.ID == comment.PostID);
                    if (!postExists)
                    {
                        return BadRequest($"Post with ID {comment.PostID} does not exist.");
                    }
                }
                else
                {
                    // Verify the parent comment exists (only for replies)
                    var parentExists = await _context.Comments.AnyAsync(c => c.ID == comment.ParentCommentID);
                    if (!parentExists)
                    {
                        return BadRequest($"Parent comment with ID {comment.ParentCommentID} does not exist.");
                    }
                }

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetComment), new { id = comment.ID }, comment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the comment: {ex.Message}");
            }
        }

        // Get a single comment by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Replies) // Include nested replies
                .FirstOrDefaultAsync(c => c.ID == id);

            if (comment == null)
            {
                return NotFound($"Comment with ID {id} not found.");
            }

            return Ok(comment);
        }

        // Get all comments and replies for a specific post
        [HttpGet("GetComments/{postId}")]
        public async Task<ActionResult<List<Comment>>> GetCommentsByPostId(int postId)
        {
            var comments = await _context.Comments
                .Where(c => c.PostID == postId && c.ParentCommentID == null) // Only top-level comments
                .Include(c => c.Replies) // Include nested replies
                .ThenInclude(r => r.Replies) // Include deeper levels of replies
                .ToListAsync();

            if (!comments.Any())
            {
                return NotFound($"No comments found for Post ID {postId}");
            }

            return Ok(comments);
        }
    }
}
