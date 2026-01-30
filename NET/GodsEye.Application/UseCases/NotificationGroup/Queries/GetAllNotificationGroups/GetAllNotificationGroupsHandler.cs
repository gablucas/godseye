using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.NotificationGroup.Queries.GetAllNotificationGroups
{
    public class GetAllNotificationGroupsHandler : IRequestHandler<GetAllNotificationGroupsRequest, ApiResponse<IEnumerable<NotificationGroupModel>>>
    {
        private readonly INotificationGroupQueryRepository _notificationQueryRepository;

        public GetAllNotificationGroupsHandler(INotificationGroupQueryRepository notificationQueryRepository)
        {
            _notificationQueryRepository = notificationQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<NotificationGroupModel>>> Handle(GetAllNotificationGroupsRequest request, CancellationToken cancellationToken)
        {
            var result = await _notificationQueryRepository.GetAll(cancellationToken);
            return ApiResponse<IEnumerable<NotificationGroupModel>>.Ok(result);
        }
    }
}
