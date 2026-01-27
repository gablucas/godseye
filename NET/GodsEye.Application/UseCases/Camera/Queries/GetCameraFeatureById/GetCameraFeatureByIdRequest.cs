using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCameraFeatureById
{
    public sealed record GetCameraFeatureByIdRequest(int cameraId) : IRequest<ApiResponse<IEnumerable<CameraFeatureModel>>>;
}
