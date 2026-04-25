using GodsEye.Application.DTOs.Model;
using GodsEye.Shared.Response.EnvironmentMonitoring;

namespace GodsEye.Application.Interfaces
{
    public interface INotificationSignalR
    {
        Task SendCreatedEnvironmentMonitoringLog(EnvironmentMonitoringLogResponse message);
        Task SendCreatedIncidentRecordingLog(IncidentRecordingModel message);
        Task SendCreatedPerson(PersonModel message);
        Task SendCreatedRoutine(RoutineModel message);
        Task SendAlertNotification(int message);
    }
}
