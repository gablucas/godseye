using GodsEye.Application.DTOs.Response;
using GodsEye.Shared.Response.AccessSchedule;
using MediatR;

namespace GodsEye.Application.UseCases.AccessSchedule.Commands.CreateAccessSchedule
{
    public sealed record CreateAccessScheduleRequest(int Id, string Name, bool IsActive, List<AccessScheduleRuleDTO> Rules) : IRequest<ApiResponse<int>>;
}
