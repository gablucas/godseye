using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCameraFeatureById
{
    public class GetCameraFeatureByIdHandler : IRequestHandler<GetCameraFeatureByIdRequest, ApiResponse<IEnumerable<CameraFeatureModel>>>
    {
        private readonly ICameraQueryRepository _cameraQueryRepository;

        public GetCameraFeatureByIdHandler(ICameraQueryRepository cameraQueryRepository)
        {
            _cameraQueryRepository = cameraQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<CameraFeatureModel>>> Handle(GetCameraFeatureByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _cameraQueryRepository.GetCameraFeaturesById(request.cameraId, cancellationToken);
            return ApiResponse<IEnumerable<CameraFeatureModel>>.Ok(result);
        }
    }
}
