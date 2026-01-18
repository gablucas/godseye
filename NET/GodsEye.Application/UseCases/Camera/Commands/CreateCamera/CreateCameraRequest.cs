using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.CreateCamera
{
    public sealed record CreateCameraRequest(string Name, string? Connection, string SectorId, IEnumerable<int> Features) : IRequest<ApiResponse<ProcedureResult>>;
}
