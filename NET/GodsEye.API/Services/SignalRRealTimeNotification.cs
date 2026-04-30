using GodsEye.API.Hubs;
using GodsEye.API.DTO;

using Microsoft.AspNetCore.SignalR;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Person;

namespace GodsEye.API.Services
{
    public class SignalRRealTimeNotification : INotificationSignalR
    {
        private readonly IHubContext<CreatedDataHub> _hub;

        public SignalRRealTimeNotification(IHubContext<CreatedDataHub> hub)
        {
            _hub = hub;
        }

        public async Task SendCreatedEnvironmentMonitoringLog(EnvironmentMonitoringDTO message)
        {
            await _hub.Clients.All.SendAsync(
            "CreatedEnvironmentMonitoring",
            message);
        }

        public async Task SendCreatedIncidentRecordingLog(IncidentRecordingDTO message)
        {
            await _hub.Clients.All.SendAsync(
            "CreatedIncidentRecording",
            message);
        }

        public async Task SendCreatedPerson(PersonResponse message)
        {
            await _hub.Clients.All.SendAsync(
            "CreatedPerson",
            message);
        }

        public async Task SendCreatedRoutine(RoutineDTO message)
        {
            await _hub.Clients.All.SendAsync(
            "CreatedRoutine",
            message);
        }

        public async Task SendAlertNotification(int message)
        {
            await _hub.Clients.All.SendAsync(
            "AlertNotification",
            message);
        }
    }
}
