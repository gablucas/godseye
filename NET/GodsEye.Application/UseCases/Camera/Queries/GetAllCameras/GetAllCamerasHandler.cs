using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetAllCameras
{
    public class GetAllCamerasHandler : IRequestHandler<GetAllCamerasRequest, ApiResponse<IEnumerable<CameraModel>>>
    {
        private readonly ICameraQueryRepository _cameraQueryRepository;

        public GetAllCamerasHandler(ICameraQueryRepository cameraQueryRepository)
        {
            _cameraQueryRepository = cameraQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<CameraModel>>> Handle(GetAllCamerasRequest request, CancellationToken cancellationToken)
        {
            var cameras = await _cameraQueryRepository.GetAll(cancellationToken);

            return ApiResponse<IEnumerable<CameraModel>>.Ok(cameras);
        }
    }
}
