using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetAllCameras
{
    public sealed record GetAllCamerasRequest : IRequest<ApiResponse<IEnumerable<CameraModel>>>;
}
