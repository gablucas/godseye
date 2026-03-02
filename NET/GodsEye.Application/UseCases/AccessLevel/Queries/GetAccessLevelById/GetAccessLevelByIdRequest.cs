using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.AccessLevel.Queries.GetAccessLevelById
{
    public sealed record GetAccessLevelByIdRequest(int AccessLevelId) : IRequest<ApiResponse<AccessLevelModel>>;
}
