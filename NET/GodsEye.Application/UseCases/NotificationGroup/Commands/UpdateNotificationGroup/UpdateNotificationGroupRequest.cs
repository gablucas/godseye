using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.NotificationGroup.Commands.UpdateNotificationGroup
{
    public sealed record UpdateNotificationGroupRequest(int Id, List<string> NewEmails, List<int> RemoveEmails) : IRequest<ApiResponse<int>>;
}
