using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.DeleteCameraRoi
{
    public sealed record DeleteCameraRoiRequest(int cameraRoiId) : IRequest<ApiResponse<int>>;
}
