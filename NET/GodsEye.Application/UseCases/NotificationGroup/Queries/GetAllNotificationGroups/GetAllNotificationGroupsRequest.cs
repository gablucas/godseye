using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.NotificationGroup.Queries.GetAllNotificationGroups
{
    public sealed record GetAllNotificationGroupsRequest() : IRequest<ApiResponse<IEnumerable<NotificationGroupModel>>>;
}
