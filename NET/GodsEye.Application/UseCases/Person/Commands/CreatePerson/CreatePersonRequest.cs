using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Commands.CreatePerson
{
    public sealed record CreatePersonRequest(string Name, byte[] Photo, int SectorId, int AccessLevelId) : IRequest<ApiResponse<ProcedureResult>>;
}
