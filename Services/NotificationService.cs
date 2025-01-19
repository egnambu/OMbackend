using Microsoft.AspNetCore.SignalR;
using OMbackend.Data;
using OMbackend.Models;

using Microsoft.AspNetCore.SignalR;
using OMbackend.Hubs;

namespace OMbackend.Services
{
    public class NotificationService
    {
        private readonly IHubContext<MessageHub> _hubContext;

        public NotificationService(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendUserNotification(string userId, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }

}