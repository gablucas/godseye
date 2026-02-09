using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.TestCameraConnection
{
    public sealed record CheckCameraConnectionRequest(string rtspUrl) : IRequest<ApiResponse<string>>;
}
