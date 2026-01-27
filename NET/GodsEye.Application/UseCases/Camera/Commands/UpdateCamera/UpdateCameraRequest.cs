using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.UpdateCamera
{
    public sealed record UpdateCameraRequest(int id, string Name, string? Connection, string SectorId, IEnumerable<int> Features) : IRequest<ApiResponse<ProcedureResult>>;
}
