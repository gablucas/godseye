using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.AccessSchedule.Commands.CreateAccessSchedule
{
    public sealed record CreateAccessScheduleRequest(int Id, string Name, bool IsActive, List<AccessScheduleRuleModel> Rules) : IRequest<ApiResponse<int>>;
}
