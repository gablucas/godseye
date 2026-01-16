using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetAllPersonEmbedding
{
    public sealed record GetAllPersonsEmbeddingRequest() : IRequest<ApiResponse<IEnumerable<PersonEmbeddingModel>>>;
}
