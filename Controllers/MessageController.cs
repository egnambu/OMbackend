using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMbackend.Data;
using OMbackend.Models;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace OMbackend.Controllers
{
    [Authorize(AuthenticationSchemes = "CookieAuth", Policy = "UserPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MessageController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }



        [HttpPost("add-from-user")]
        public async Task<IActionResult> AddMessageFromUser([FromBody] Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Content))
                return BadRequest("Message content is required.");

            // Validate foreign keys
            if (!await _context.Users.AnyAsync(u => u.ID == message.UserID))
                return NotFound("User not found.");
            if (!await _context.Shops.AnyAsync(s => s.ID == message.ShopID))
                return NotFound("Shop not found.");
            if (message.ConversationID.HasValue &&
                !await _context.Conversations.AnyAsync(c => c.ID == message.ConversationID))
                return NotFound("Conversation not found.");

            // Indicate the message is from a user
            message.IsUser = true;
            message.CreatedAt = DateTime.UtcNow;

            // Add the message to the database
            _context.Messages.Add(message);

            // Update the IsSeen property of the conversation
            if (message.ConversationID.HasValue)
            {
                var conversation = await _context.Conversations.FindAsync(message.ConversationID.Value);
                if (conversation != null)
                {
                    conversation.IsAdminSeen = false;
                    conversation.LastUpdatedAt = DateTime.Now;
                    _context.Conversations.Update(conversation);
                }
            }

            await _context.SaveChangesAsync();

            var messageDto = _mapper.Map<MessageDto>(message);
            return CreatedAtAction(nameof(GetMessagesByConversation), new { conversationID = message.ConversationID }, messageDto);
        }




        [HttpPost("add-from-shop")]
        public async Task<IActionResult> AddMessageFromShop([FromBody] Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Content))
                return BadRequest("Message content is required.");

            // Validate foreign keys
            if (!await _context.Shops.AnyAsync(s => s.ID == message.ShopID))
                return NotFound("Shop not found.");
            if (!await _context.Users.AnyAsync(u => u.ID == message.UserID))
                return NotFound("User not found.");
            if (message.ConversationID.HasValue &&
                !await _context.Conversations.AnyAsync(c => c.ID == message.ConversationID))
                return NotFound("Conversation not found.");

            // Indicate the message is from a shop
            message.IsUser = false;
            message.CreatedAt = DateTime.UtcNow;

            // Add the message to the database
            _context.Messages.Add(message);

            // Update the IsSeen property of the conversation
            if (message.ConversationID.HasValue)
            {
                var conversation = await _context.Conversations.FindAsync(message.ConversationID.Value);
                if (conversation != null)
                {
                    conversation.IsSeen = false;
                    conversation.LastUpdatedAt = DateTime.Now;
                    _context.Conversations.Update(conversation);
                }
            }

            // Ensure changes are saved
            await _context.SaveChangesAsync();

            // Log success for debugging
            Console.WriteLine("IsSeen property updated successfully.");

            // Map and return response
            var messageDto = _mapper.Map<MessageDto>(message);
            return CreatedAtAction(nameof(GetMessagesByConversation), new { conversationID = message.ConversationID }, messageDto);
        }



        [HttpGet("conversation/{conversationID}/new")]
        public async Task<IActionResult> GetNewMessages(int conversationID, DateTime lastCheck)
        {
            var newMessages = await _context.Messages
                .Where(m => m.ConversationID == conversationID && m.CreatedAt > lastCheck)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            if (newMessages == null || newMessages.Count == 0)
                return NoContent();

            var messageDtos = _mapper.Map<List<MessageDto>>(newMessages);
            return Ok(messageDtos);
        }




        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateMessage(int id, [FromBody] string newContent)
        {
            if (string.IsNullOrEmpty(newContent))
                return BadRequest("New message content is required.");

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
                return NotFound("Message not found.");

            message.Content = newContent;
            await _context.SaveChangesAsync();

            var messageDto = _mapper.Map<MessageDto>(message);
            return Ok(new { Message = "Message updated successfully.", MessageData = messageDto });
        }

        

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
                return NotFound("Message not found.");

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Message deleted successfully." });
        }


        
        [HttpGet("user/{userID}/shop/{shopID}")]
        public async Task<IActionResult> GetMessagesByUserAndShop(int userID, int shopID)
        {
            var messages = await _context.Messages
                .Where(m => m.UserID == userID && m.ShopID == shopID)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            if (!messages.Any())
                return NotFound("No messages found for this user and shop.");

            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);
            return Ok(messageDtos);
        }



        [HttpGet("conversation/{conversationID}")]
        public async Task<IActionResult> GetMessagesByConversation(int conversationID)
        {
            var messages = await _context.Messages
                .Where(m => m.ConversationID == conversationID)
                .OrderBy(m => m.CreatedAt) // Order by CreatedAt in ascending order
                .ToListAsync();

            if (!messages.Any())
                return NotFound("No messages found for this conversation.");

            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);
            return Ok(messageDtos);
        }
    }
}
