using GodsEye.API.DTO;

namespace GodsEye.API.Interfaces
{
    public interface INotificationSignalR
    {
        Task SendCreatedEnvironmentMonitoringLog(EnvironmentMonitoringDTO message);
        Task SendCreatedIncidentRecordingLog(IncidentRecordingDTO message);
        Task SendCreatedPerson(PersonDTO message);
        Task SendCreatedRoutine(RoutineDTO message);
        Task SendAlertNotification(int message);
    }
}
