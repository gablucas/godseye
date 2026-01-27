using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetCameraById
{
    public class GetCameraByIdHandler : IRequestHandler<GetCameraByIdRequest, ApiResponse<CameraModel>>
    {
        private readonly ICameraQueryRepository _cameraQueryRepository;

        public GetCameraByIdHandler(ICameraQueryRepository cameraQueryRepository)
        {
            _cameraQueryRepository = cameraQueryRepository;
        }

        public async Task<ApiResponse<CameraModel>> Handle(GetCameraByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _cameraQueryRepository.GetById(request.cameraId, cancellationToken);
            return ApiResponse<CameraModel>.Ok(result);
        }
    }
}
