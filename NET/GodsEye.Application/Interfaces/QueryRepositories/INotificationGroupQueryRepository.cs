using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.QueryRepositories
{
    public interface INotificationGroupQueryRepository
    {
        Task<IEnumerable<NotificationGroupModel>> GetAll(CancellationToken cancellationToken);
    }
}
