using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.AccessSchedule.Queries.GetAllAccessSchedules
{
    public sealed record GetAllAccessSchedulesRequest() : IRequest<ApiResponse<IEnumerable<AccessScheduleModel>>>;
}
