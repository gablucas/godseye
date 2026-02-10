using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.Enums;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.CreateCameraRoi
{
    public sealed record CreateCameraRoiRequest(int CameraId, RoiTypeEnum RoiType, RoiModel Coordinates) : IRequest<ApiResponse<int>>;
}
