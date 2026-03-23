using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Commands.CreateRecognize
{
    public sealed record CreateRecognizeRequest(int PersonId, byte[] Photo) : IRequest<ApiResponse<ProcedureResult>>;
}
