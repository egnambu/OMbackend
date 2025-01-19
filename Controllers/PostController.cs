using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMbackend.Data;
using OMbackend.Models;
using static OMbackend.Models.PostDTO;

namespace OMbackend.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PostController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("create")]
        public async Task<ActionResult<PostResponseDto>> CreatePost([FromBody] PostCreateDto postDto)
        {
            if (postDto == null)
            {
                return BadRequest("Post data is required.");
            }

            var post = _mapper.Map<Post>(postDto);

            try
            {
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                var responseDto = _mapper.Map<PostResponseDto>(post);
                return CreatedAtAction(nameof(GetPost), new { id = post.ID }, responseDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the post: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PostResponseDto>> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound($"Post with ID {id} not found.");
            }

            var responseDto = _mapper.Map<PostResponseDto>(post);
            return Ok(responseDto);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<PostResponseDto>>> GetAllPosts()
        {
            var posts = await _context.Posts.ToListAsync();

            if (!posts.Any())
            {
                return NotFound("No posts found.");
            }

            var responseDtos = _mapper.Map<List<PostResponseDto>>(posts);
            return Ok(responseDtos);
        }


    }
}
