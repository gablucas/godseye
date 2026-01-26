using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCamerasByFeatureID
{
    public sealed record GetCamerasByFeatureIdRequest(int featureId) : IRequest<ApiResponse<IEnumerable<CameraByFeatureModel>>>;
}
