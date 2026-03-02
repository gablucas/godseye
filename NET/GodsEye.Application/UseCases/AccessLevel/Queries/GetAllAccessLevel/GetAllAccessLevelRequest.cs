using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.AccessLevel.Queries.GetAllAcessLevel
{
    public sealed record GetAllAccessLevelRequest() : IRequest<ApiResponse<IEnumerable<AccessLevelModel>>>;
}
