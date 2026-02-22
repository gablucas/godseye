using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.AccessSchedule.Queries.GetAccessSchedulesById
{
    public sealed record GetAccessSchedulesByIdRequest(int AccessScheduleId) : IRequest<ApiResponse<AccessScheduleModel>>;
}
