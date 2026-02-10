using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCamerasRoiByCameraId
{
    public sealed record GetCamerasRoiByCameraIdRequest(int cameraId) : IRequest<ApiResponse<List<CameraRoiModel>>>;
}
