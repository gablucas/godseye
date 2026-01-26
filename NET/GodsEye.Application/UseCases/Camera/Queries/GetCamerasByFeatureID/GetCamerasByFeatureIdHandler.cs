using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCamerasByFeatureID
{
    public class GetCamerasByFeatureIdHandler : IRequestHandler<GetCamerasByFeatureIdRequest, ApiResponse<IEnumerable<CameraByFeatureModel>>>
    {
        private readonly ICameraQueryRepository _cameraQueryRepository;

        public GetCamerasByFeatureIdHandler(ICameraQueryRepository cameraQueryRepository)
        {
            _cameraQueryRepository = cameraQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<CameraByFeatureModel>>> Handle(GetCamerasByFeatureIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _cameraQueryRepository.GetCamerasByFeatureId(request.featureId, cancellationToken);
            return ApiResponse<IEnumerable<CameraByFeatureModel>>.Ok(result);
        }
    }
}
