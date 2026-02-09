using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Queries.TestCameraConnection
{
    public class CheckCameraConnectionHandler : IRequestHandler<CheckCameraConnectionRequest, ApiResponse<string>>
    {
        private readonly ICameraConnectionTesterService _cameraConnectionTestService;
        private readonly IMediaMtxService _mediaMtxService;

        public CheckCameraConnectionHandler(ICameraConnectionTesterService cameraConnectionTestService, IMediaMtxService mediaMtxService)
        {
            _cameraConnectionTestService = cameraConnectionTestService;
            _mediaMtxService = mediaMtxService;
        }

        public async Task<ApiResponse<string>> Handle(CheckCameraConnectionRequest request, CancellationToken cancellationToken)
        {
            var (isOnline, message) = await _cameraConnectionTestService.TestConnectionAsync(request.rtspUrl);

            if (!isOnline)
                return ApiResponse<string>.Fail(500, message);
          
            return ApiResponse<string>.Ok("Conexão online");
        }
    }
}
