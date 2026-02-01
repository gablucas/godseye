using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetPersonById
{
    public sealed record GetPersonByIdRequest(int personId) : IRequest<ApiResponse<PersonModel>>;
}
