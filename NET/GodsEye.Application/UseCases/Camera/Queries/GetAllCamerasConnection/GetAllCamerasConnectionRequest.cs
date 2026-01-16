using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetAllCamerasConnection
{
    public sealed record GetAllCamerasConnectionRequest() : IRequest<ApiResponse<IEnumerable<CameraConnectionModel>>>;
}
