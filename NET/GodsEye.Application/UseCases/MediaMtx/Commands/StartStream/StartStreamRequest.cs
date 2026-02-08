using GodsEye.Application.DTOs.Response;
using MediatR;

namespace GodsEye.Application.UseCases.MediaMtx.Commands.StartStream
{
    public sealed record StartStreamRequest(string RtspUrl) : IRequest<ApiResponse<string>>;
}