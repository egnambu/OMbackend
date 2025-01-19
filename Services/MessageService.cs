using Microsoft.AspNetCore.SignalR;
using OMbackend.Models; // Adjust namespaces as needed
using OMbackend.Hubs;
using System.Threading.Tasks;
using OMbackend.Data;
using Microsoft.AspNetCore.Authorization;

namespace OMbackend.Services
{
    public class MessageService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly MessageHub _messageHub;

        public MessageService(AppDbContext context, IHubContext<MessageHub> hubContext, MessageHub messageHub)
        {
            _context = context;
            _hubContext = hubContext;
            _messageHub = messageHub;
        }

            [Authorize(AuthenticationSchemes = "CookieAuth", Policy = "UserPolicy")]
            public async Task MessageShop(int userId, int shopId, string content, int? conversationId)
            {
                // Create a new message
                var message = new Message
                {
                    UserID = userId,
                    ShopID = shopId,
                    Content = content,
                    CreatedAt = DateTime.Now,
                    ConversationID = conversationId,
                    IsUser = userId == 12  // Check if the sender is the special user (ID = 12)
                };

                // Add the message to the database
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // Check if the recipient (Shop) is online using MessageHub
                var conversation = await _context.Conversations.FindAsync(conversationId);
                if (conversation != null)
                {
                    // If the user is online, set IsSeen to true, otherwise set to false
                    if (_messageHub.IsUserOnline(shopId.ToString()))
                    {
                        conversation.IsSeen = true;
                    }
                    else
                    {
                        conversation.IsSeen = false;
                    }

                    // Save changes to the Conversation entity
                    await _context.SaveChangesAsync();
                }

                // Optionally send the message to the shop if they are online
                if (_messageHub.IsUserOnline(shopId.ToString()))
                {
                    await _hubContext.Clients.User(shopId.ToString()).SendAsync("ReceiveMessage", content);
                }
            }
        

            [Authorize(AuthenticationSchemes = "CookieAuth", Policy = "UserPolicy")]
            public async Task MessageUser(int userId, int shopId, string content, int? conversationId)
            {
                // Create a new message
                var message = new Message
                {
                    UserID = userId,
                    ShopID = shopId,
                    Content = content,
                    CreatedAt = DateTime.Now,
                    ConversationID = conversationId,
                    IsUser = userId == 12  // Check if the sender is the special user (ID = 12)
                };

                // Add the message to the database
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // Check if the recipient (User) is online using MessageHub
                var conversation = await _context.Conversations.FindAsync(conversationId);
                if (conversation != null)
                {
                    // If the user is online, set IsSeen to true, otherwise set to false
                    if (_messageHub.IsUserOnline(userId.ToString()))
                    {
                        conversation.IsSeen = true;
                    }
                    else
                    {
                        conversation.IsSeen = false;
                    }

                    // Save changes to the Conversation entity
                    await _context.SaveChangesAsync();
                }

                // Optionally send the message to the shop if they are online
                if (_messageHub.IsUserOnline(userId.ToString()))
                {
                    await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", content);
                }
            }
        }
    }



