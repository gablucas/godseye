using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;

namespace GodsEye.Domain.Interfaces.Repositories
{
    public interface INotificationGroupRepository
    {
        Task<ProcedureResult> Create(NotificationGroupEntity entity, CancellationToken cancellationToken);
    }
}
