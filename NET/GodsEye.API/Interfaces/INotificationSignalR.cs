
using GodsEye.API.DTO;
using GodsEye.Shared.Response;
using GodsEye.Shared.Response.Person;

namespace GodsEye.API.Interfaces
{
    public interface INotificationSignalR
    {
        Task SendCreatedEnvironmentMonitoringLog(EnvironmentMonitoringDTO message);
        Task SendCreatedIncidentRecordingLog(IncidentRecordingDTO message);
        Task SendCreatedComplianceViolationLog(int message);
        Task SendCreatedPerson(PersonResponse message);
        Task SendAlertNotification(ViolationAlertFeatureResponse message);
    }
}
