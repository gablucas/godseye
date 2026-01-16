using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.GodsEye.Commands.StartStream
{
    public sealed record StartStreamRequest(string name, string url) : IRequest<ApiResponse<CameraPreviewResponse>>;
}
