using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetAllPersons
{
    public sealed record GetAllPersonRequest() : IRequest<ApiResponse<IEnumerable<PersonModel>>>;
}
