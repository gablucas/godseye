using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.NotificationGroup.Commands.CreateNotificationGroup
{
    public sealed record CreateNotificationGroupRequest(string name, IEnumerable<string> emails) : IRequest<ApiResponse<ProcedureResult>>;
}
