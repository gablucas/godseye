using GodsEye.API.Hubs;
using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GodsEye.API.Services
{
    public class SignalRRealTimeNotification : INotificationSignalR
    {
        private readonly IHubContext<CreatedDataHub> _hub;

        public SignalRRealTimeNotification(IHubContext<CreatedDataHub> hub)
        {
            _hub = hub;
        }

        public async Task SendCreatedEnvironmentMonitoringLog(EnvironmentMonitoringModel message)
        {
            await _hub.Clients.All.SendAsync(
            "CreatedEnvironmentMonitoring",
            message);
        }

        public async Task SendCreatedIncidentRecordingLog(IncidentRecordingModel message)
        {
            await _hub.Clients.All.SendAsync(
            "CreatedIncidentRecording",
            message);
        }

        public async Task SendCreatedPerson(PersonModel message)
        {
            await _hub.Clients.All.SendAsync(
            "CreatedPerson",
            message);
        }
    }
}
