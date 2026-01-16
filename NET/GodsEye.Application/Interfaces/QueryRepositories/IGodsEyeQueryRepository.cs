using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.QueryRepositories
{
    public interface IGodsEyeQueryRepository
    {
        Task<MonitoringDataModel> GetMonitoringData();
    }
}
