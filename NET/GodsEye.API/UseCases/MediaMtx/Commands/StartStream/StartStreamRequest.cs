using MediatR;

namespace GodsEye.API.UseCases.MediaMtx.Commands.StartStream
{
    public sealed record StartStreamRequest(string RtspUrl) : IRequest<string>;
}