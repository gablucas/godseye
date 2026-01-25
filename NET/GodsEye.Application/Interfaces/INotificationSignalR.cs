using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces
{
    public interface INotificationSignalR
    {
        Task SendEnvironmentMonitoringCreatedLog(EnvironmentMonitoringModel message);
    }
}
