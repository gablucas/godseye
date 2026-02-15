using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.UpdateCameraRoi
{
    public sealed record UpdateCameraRoiRequest(int CameraRoiId, RoiModel Coordinates, bool IsActive) : IRequest<ApiResponse<int>>;

}
