using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetPersonLog
{
    public sealed record GetPersonLogRequest(int personId) : IRequest<ApiResponse<IEnumerable<PersonLogModel>>>;
}
