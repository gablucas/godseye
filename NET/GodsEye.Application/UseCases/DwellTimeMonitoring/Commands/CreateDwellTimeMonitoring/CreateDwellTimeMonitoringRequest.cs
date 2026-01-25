using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.DwellTimeMonitoring.Commands.CreateDwellTimeMonitoring
{
    public sealed record CreateDwellTimeMonitoringRequest(int personId, int cameraId) : IRequest<ApiResponse<ProcedureResult>>;
}
