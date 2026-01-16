using GodsEye.API.Hubs;
using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GodsEye.API.Services
{
    public class SignalRRealTimeNotification : INotificationSignalR
    {
        private readonly IHubContext<NotificationsHub> _hub;

        public SignalRRealTimeNotification(IHubContext<NotificationsHub> hub)
        {
            _hub = hub;
        }

        public async Task SendCreatedLog(EnvironmentMonitoringModel message)
        {
            await _hub.Clients.All.SendAsync(
            "ReceiveMessage",
            message);
        }
    }
}
