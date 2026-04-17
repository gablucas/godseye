using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces
{
    public interface INotificationSignalR
    {
        Task SendCreatedEnvironmentMonitoringLog(EnvironmentMonitoringModel message);
        Task SendCreatedIncidentRecordingLog(IncidentRecordingModel message);
        Task SendCreatedPerson(PersonModel message);
        Task SendCreatedRoutine(RoutineModel message);
        Task SendAlertNotification(int message);
    }
}
