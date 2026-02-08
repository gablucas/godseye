using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.MediaMtx.Commands.StartStream
{
    public class StartStreamHandler : IRequestHandler<StartStreamRequest, ApiResponse<string>>
    {
        private IMediaMtxService _mediaMtxService;
        private ICameraConnectionTesterService _cameraConnectionTesterService;

        public StartStreamHandler(IMediaMtxService mediaMtxService, ICameraConnectionTesterService cameraConnectionTesterService)
        {
            _mediaMtxService = mediaMtxService;
            _cameraConnectionTesterService = cameraConnectionTesterService;
        }

        public async Task<ApiResponse<string>> Handle(StartStreamRequest request, CancellationToken cancellationToken)
        {
            var (isValid, message) = await _cameraConnectionTesterService.TestConnectionAsync(request.RtspUrl);

            if (!isValid)
            {
                return ApiResponse<string>.Fail(400, message);
            }

            var webRtcUrl = await _mediaMtxService.StartStream(request.RtspUrl);


            return ApiResponse<string>.Ok(webRtcUrl);
        }
    }
}
