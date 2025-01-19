using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OMbackend.Hubs
{
    [Authorize(AuthenticationSchemes = "CookieAuth", Policy = "UserPolicy")]
    public class MessageHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> OnlineUsers = new();

        public override async Task OnConnectedAsync()
        {
            try
            {
                // Get the UserID from the claims
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentNullException("User Identifier cannot be null or empty.");
                }

                // Add the user to the online dictionary
                OnlineUsers[userId] = Context.ConnectionId;

                Console.WriteLine($"User connected: {userId}, ConnectionId: {Context.ConnectionId}");
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnConnectedAsync: {ex.Message}");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    OnlineUsers.TryRemove(userId, out _);
                    Console.WriteLine($"User disconnected: {userId}");
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnDisconnectedAsync: {ex.Message}");
                throw;
            }
        }

        public bool IsUserOnline(string userId)
        {
            return OnlineUsers.ContainsKey(userId);
        }

        public async Task BroadcastMessage(string shopId, string messageContent)
        {
            try
            {
                var isOnline = IsUserOnline(shopId);
                if (isOnline)
                {
                    await Clients.User(shopId).SendAsync("ReceiveMessage", messageContent);
                    Console.WriteLine($"Message sent to shop {shopId}: {messageContent}");
                }
                else
                {
                    Console.WriteLine($"Shop {shopId} is offline. Storing the message.");
                    // Logic for storing the message can be added here.
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting message: {ex.Message}");
                throw;
            }
        }
    }
}
