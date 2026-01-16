using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Person.Queries.GetCameraLog
{
    public class GetCameraLogHandler : IRequestHandler<GetCameraLogRequest, ApiResponse<IEnumerable<CameraLogModel>>>
    {
        private readonly ICameraQueryRepository _cameraQueryRepository;

        public GetCameraLogHandler(ICameraQueryRepository cameraQueryRepository)
        {
            _cameraQueryRepository = cameraQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<CameraLogModel>>> Handle(GetCameraLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _cameraQueryRepository.GetAllLogs(request.cameraId, cancellationToken);
            return ApiResponse<IEnumerable<CameraLogModel>>.Ok(result);
        }
    }
}
