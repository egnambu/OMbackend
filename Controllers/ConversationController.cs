using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMbackend.Data;
using OMbackend.Models;
using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;

namespace OMbackend.Controllers
{

    [Authorize(AuthenticationSchemes = "CookieAuth", Policy = "UserPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ConversationController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Add a new conversation
        [HttpPost("add")]
        public async Task<IActionResult> AddConversation([FromBody] Conversation conversation)
        {
            if (conversation == null)
                return BadRequest("Conversation data is required.");

            // Validate foreign keys
            if (!await _context.Users.AnyAsync(u => u.ID == conversation.UserID))
                return NotFound("User not found.");
            if (!await _context.Shops.AnyAsync(s => s.ID == conversation.ShopID))
                return NotFound("Shop not found.");

            // Check if a conversation already exists between the user and the shop
            var existingConversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.UserID == conversation.UserID && c.ShopID == conversation.ShopID);

            if (existingConversation != null)
                return Conflict("A conversation already exists between the user and the shop.");

            // Add the new conversation
            conversation.CreatedAt = DateTime.UtcNow;
            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            var conversationDto = _mapper.Map<ConversationDto>(conversation);
            return CreatedAtAction(nameof(GetConversationsByUser), new { userID = conversation.UserID }, conversationDto);
        }

        // Get all conversations for a specific user
        [HttpGet("user/{userID}")]
        public async Task<IActionResult> GetConversationsByUser(int userID)
        {
            var userExists = await _context.Users.AnyAsync(u => u.ID == userID);
            if (!userExists)
                return NotFound("User not found.");

            var conversations = await _context.Conversations
                .Where(c => c.UserID == userID)
                .Include(c => c.Shop)  // Ensure that Shop is included for the name mapping
                .OrderByDescending(c => c.LastUpdatedAt)  // Order by latest conversation
                .ToListAsync();

            // Log the conversations to inspect their IDs (optional)
            foreach (var conversation in conversations)
            {
                Console.WriteLine($"Conversation ID: {conversation.ID}, Shop Name: {conversation.Shop?.Name}, IsSeen: {conversation.IsSeen}");
            }

            // Map the conversations to ConversationDto, including the IsSeen value
            var conversationDtos = conversations.Select(c => new ConversationDto
            {
                ConversationId = c.ID, // Explicitly map ConversationId to ID
                Name = c.Shop.Name, // Assuming Shop.Name exists
                ShopId = c.ShopID,
                UserID = c.UserID,
                IsSeen = c.IsSeen, // Ensure IsSeen is mapped properly
                CreatedAt = c.CreatedAt,
                LastUpdatedAt = c.LastUpdatedAt
            }).ToList();

            return Ok(conversationDtos);
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllConversations()
        {
            // Fetch all conversations with User and Shop details
            var conversations = await _context.Conversations
                .Include(c => c.User) // Include User data for reference
                .Include(c => c.Shop) // Include Shop data for reference
                .OrderByDescending(c => c.LastUpdatedAt) // Order by LastUpdatedAt
                .ToListAsync();

            // Return the raw data directly
            return Ok(conversations);
        }



        // Get all conversations for a specific shop
        [HttpGet("shop/{shopID}")]
        public async Task<IActionResult> GetConversationsByShop(int shopID)
        {
            var shopExists = await _context.Shops.AnyAsync(s => s.ID == shopID);
            if (!shopExists)
                return NotFound("Shop not found.");

            var conversations = await _context.Conversations
                .Where(c => c.ShopID == shopID)
                .Include(c => c.User)
                .OrderByDescending(c => c.LastUpdatedAt)
                .ToListAsync();

            // Map the conversations to ConversationDto, including the IsSeen value
            var conversationDtos = conversations.Select(c => new ConversationDto
            {
                ID = c.ID,
                UserID = c.UserID,
                ShopId = c.ShopID,
                IsSeen = c.IsSeen,
                CreatedAt = c.CreatedAt,
                LastUpdatedAt = c.LastUpdatedAt
            }).ToList();

            return Ok(conversationDtos);
        }


        [HttpGet("{conversationID}/last-message")]
        public async Task<IActionResult> GetLastMessageInConversation(int conversationID)
        {
            // Check if the conversation exists
            var conversationExists = await _context.Conversations.AnyAsync(c => c.ID == conversationID);
            if (!conversationExists)
                return NotFound("Conversation not found.");

            // Get the last message for the given conversation
            var lastMessage = await _context.Messages
                .Where(m => m.ConversationID == conversationID)
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            if (lastMessage == null)
                return NotFound("No messages found in this conversation.");

            // Map to MessageDto
            var lastMessageDto = _mapper.Map<MessageDto>(lastMessage);

            return Ok(lastMessageDto);
        }



        // Get messages for a specific conversation
        [HttpGet("{conversationID}/messages")]
        public async Task<IActionResult> GetMessagesForConversation(int conversationID)
        {
            var conversationExists = await _context.Conversations.AnyAsync(c => c.ID == conversationID);
            if (!conversationExists)
                return NotFound("Conversation not found.");

            var messages = await _context.Messages
                .Where(m => m.ConversationID == conversationID)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return Ok(messages);
        }



        [HttpPut("{conversationID}/set-is-seen")]
        public async Task<IActionResult> SetIsSeenToFalse(int conversationID)
        {
            // Check if the conversation exists
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.ID == conversationID);

            if (conversation == null)
                return NotFound("Conversation not found.");

            // Set IsSeen to false
            conversation.IsSeen = true;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { Message = "IsSeen status has been set to false." });
        }


        [HttpPut("{conversationID}/set-is-adminseen")]
        public async Task<IActionResult> SetIsAdminSeenFalse(int conversationID)
        {
            // Log the incoming conversationID for debugging
            Console.WriteLine($"Received conversationID: {conversationID}");

            // Check if the conversation exists
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.ID == conversationID);

            if (conversation == null)
            {
                return NotFound(new { Error = "Conversation not found." });
            }

            // Set IsAdminSeen to true
            conversation.IsAdminSeen = true;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { Message = "IsSeen status has been updated." });
        }


        // Delete a conversation by ID
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteConversation(int id)
        {
            var conversation = await _context.Conversations.FindAsync(id);
            if (conversation == null)
                return NotFound("Conversation not found.");

            // Delete related messages first
            var relatedMessages = _context.Messages.Where(m => m.ConversationID == id);
            _context.Messages.RemoveRange(relatedMessages);

            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Conversation and related messages deleted successfully." });
        }


        [HttpGet("unseen-admin")]
        public async Task<IActionResult> GetUnseenConversationsForAdmin()
        {
            // Fetch all conversations where IsAdminSeen is false
            var unseenConversations = await _context.Conversations
                .Include(c => c.User)
                .Include(c => c.Shop) 
                .Where(c => !c.IsAdminSeen)
                .OrderByDescending(c => c.LastUpdatedAt)
                .ToListAsync();

            // Return the filtered conversations
            return Ok(unseenConversations);
        }


    }

}
