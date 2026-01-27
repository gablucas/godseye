using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCameraById
{
    public sealed record GetCameraByIdRequest(int cameraId): IRequest<ApiResponse<CameraModel>>;
}
