using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Sector.Commands.CreateSector
{
    public sealed record CreateSectorRequest(string Name, IEnumerable<string> NotificationGroups) : IRequest<ApiResponse<ProcedureResult>>;
}
