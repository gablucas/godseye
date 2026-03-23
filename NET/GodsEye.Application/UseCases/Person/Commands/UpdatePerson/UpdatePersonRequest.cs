using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Commands.UpdatePerson
{
    public sealed record UpdatePersonRequest(int Id, string Name, int SectorId, int AccessLevelId) : IRequest<ApiResponse<ProcedureResult>>;
}
