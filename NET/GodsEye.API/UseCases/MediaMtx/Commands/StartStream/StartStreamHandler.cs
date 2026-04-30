using GodsEye.API.Interfaces;
using MediatR;

namespace GodsEye.API.UseCases.MediaMtx.Commands.StartStream
{
    public class StartStreamHandler : IRequestHandler<StartStreamRequest, string>
    {
        private IMediaMtxService _mediaMtxService;
        private ICameraConnectionTesterService _cameraConnectionTesterService;

        public StartStreamHandler(IMediaMtxService mediaMtxService, ICameraConnectionTesterService cameraConnectionTesterService)
        {
            _mediaMtxService = mediaMtxService;
            _cameraConnectionTesterService = cameraConnectionTesterService;
        }

        public async Task<string?> Handle(StartStreamRequest request, CancellationToken cancellationToken)
        {
            var (isValid, message) = await _cameraConnectionTesterService.TestConnectionAsync(request.RtspUrl);

            if (!isValid)
            {
                return null;
            }

            var webRtcUrl = await _mediaMtxService.StartStream(request.RtspUrl);


            return webRtcUrl;
        }
    }
}
