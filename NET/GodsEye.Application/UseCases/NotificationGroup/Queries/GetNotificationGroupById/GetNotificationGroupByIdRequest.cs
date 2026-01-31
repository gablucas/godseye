using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.NotificationGroup.Queries.GetNotificationGroupById
{
    public sealed record GetNotificationGroupByIdRequest(int Id) : IRequest<ApiResponse<NotificationGroupModel>>;
}
