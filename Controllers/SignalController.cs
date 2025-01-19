using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OMbackend.Data;
using OMbackend.Models;
using OMbackend.Hubs;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace OMbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignalController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly MessageHub _messageHub;

        public SignalController(AppDbContext context, IHubContext<MessageHub> hubContext, MessageHub messageHub)
        {
            _context = context;
            _hubContext = hubContext;
            _messageHub = messageHub;
        }

        [HttpPost("add-from-user")]
        public async Task<IActionResult> AddMessageFromUser([FromBody] Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Content))
                return BadRequest("Message content is required.");

            if (!await _context.Users.AnyAsync(u => u.ID == message.UserID))
                return NotFound("User not found.");
            if (!await _context.Shops.AnyAsync(s => s.ID == message.ShopID))
                return NotFound("Shop not found.");
            if (message.ConversationID.HasValue &&
                !await _context.Conversations.AnyAsync(c => c.ID == message.ConversationID))
                return NotFound("Conversation not found.");

            message.IsUser = true;
            message.CreatedAt = DateTime.UtcNow;

            // Check if the shop (recipient) is online
            bool isShopOnline = _messageHub.IsUserOnline(message.ShopID.ToString());

            if (isShopOnline)
            {
                // Notify the shop in real-time
                await _hubContext.Clients.User(message.ShopID.ToString()).SendAsync("ReceiveMessage", new
                {
                    message.Content,
                    message.IsUser,
                    message.CreatedAt
                });
            }

            // Store the message in the database
            _context.Messages.Add(message);

            if (message.ConversationID.HasValue)
            {
                var conversation = await _context.Conversations.FindAsync(message.ConversationID.Value);
                if (conversation != null)
                {
                    conversation.IsSeen = isShopOnline; // Mark as seen if the shop is online
                    conversation.LastUpdatedAt = DateTime.UtcNow;
                    _context.Conversations.Update(conversation);
                }
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMessagesByConversation), new { conversationID = message.ConversationID }, message);
        }

        [HttpPost("add-from-shop")]
        public async Task<IActionResult> AddMessageFromShop([FromBody] Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Content))
                return BadRequest("Message content is required.");

            if (!await _context.Shops.AnyAsync(s => s.ID == message.ShopID))
                return NotFound("Shop not found.");
            if (!await _context.Users.AnyAsync(u => u.ID == message.UserID))
                return NotFound("User not found.");
            if (message.ConversationID.HasValue &&
                !await _context.Conversations.AnyAsync(c => c.ID == message.ConversationID))
                return NotFound("Conversation not found.");

            message.IsUser = false;
            message.CreatedAt = DateTime.UtcNow;

            // Check if the user (recipient) is online
            bool isUserOnline = _messageHub.IsUserOnline(message.UserID.ToString());

            if (isUserOnline)
            {
                // Notify the user in real-time
                await _hubContext.Clients.User(message.UserID.ToString()).SendAsync("ReceiveMessage", new
                {
                    message.Content,
                    message.IsUser,
                    message.CreatedAt
                });
            }

            // Store the message in the database
            _context.Messages.Add(message);

            if (message.ConversationID.HasValue)
            {
                var conversation = await _context.Conversations.FindAsync(message.ConversationID.Value);
                if (conversation != null)
                {
                    conversation.IsSeen = isUserOnline; // Mark as seen if the user is online
                    conversation.LastUpdatedAt = DateTime.UtcNow;
                    _context.Conversations.Update(conversation);
                }
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMessagesByConversation), new { conversationID = message.ConversationID }, message);
        }

        [HttpGet("conversation/{conversationID}")]
        public async Task<IActionResult> GetMessagesByConversation(int conversationID)
        {
            var messages = await _context.Messages
                .Where(m => m.ConversationID == conversationID)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            if (!messages.Any())
                return NotFound("No messages found for this conversation.");

            return Ok(messages);
        }
    }
}
