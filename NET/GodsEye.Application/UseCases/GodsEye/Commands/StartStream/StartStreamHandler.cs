using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.GodsEye.Commands.StartStream
{
    //public class StartStreamHandler : IRequestHandler<StartStreamRequest, ApiResponse<CameraPreviewResponse>>
    //{
    //    private readonly IGodsEyeService _godEeyeService;

    //    public StartStreamHandler(IGodsEyeService godEeyeService)
    //    {
    //        _godEeyeService = godEeyeService;
    //    }

    //    public async Task<ApiResponse<CameraPreviewResponse>> Handle(StartStreamRequest request, CancellationToken cancellationToken)
    //    {
    //        var result = await _godEeyeService.StartStream(request.name, request.url);

    //        return ApiResponse<CameraPreviewResponse>.Ok(result);
    //    }
    //}
}
