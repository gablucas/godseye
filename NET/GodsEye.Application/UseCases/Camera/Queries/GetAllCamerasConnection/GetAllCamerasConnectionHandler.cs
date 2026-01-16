using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.GetAllCamerasConnection
{
    public class GetAllCamerasConnectionHandler : IRequestHandler<GetAllCamerasConnectionRequest, ApiResponse<IEnumerable<CameraConnectionModel>>>
    {
        private readonly ICameraQueryRepository _cameraQueryRespository;

        public GetAllCamerasConnectionHandler(ICameraQueryRepository cameraQueryRespository)
        {
            _cameraQueryRespository = cameraQueryRespository;
        }

        public async Task<ApiResponse<IEnumerable<CameraConnectionModel>>> Handle(GetAllCamerasConnectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _cameraQueryRespository.GetAllConnection(cancellationToken);
            return ApiResponse<IEnumerable<CameraConnectionModel>>.Ok(result);
        }
    }
}
