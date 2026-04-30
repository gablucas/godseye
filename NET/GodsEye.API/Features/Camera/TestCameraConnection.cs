using GodsEye.API.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Camera
{
    public sealed record TestCameraConnectionCommand(string rtspUrl) : IRequest<string>;

    internal sealed record TestCameraConnectionHandler(ICameraConnectionTesterService cameraConnectionTestService, IMediaMtxService mediaMtxService) : IRequestHandler<TestCameraConnectionCommand, string>
    {
        public async Task<string> Handle(TestCameraConnectionCommand request, CancellationToken cancellationToken)
        {
            var (isOnline, message) = await cameraConnectionTestService.TestConnectionAsync(request.rtspUrl);

            if (!isOnline)
                return message;

            return "Conexão online";
        }
    }

    public class TestCameraConnectionEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/camera/test-connection", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] TestCameraConnectionCommand request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(request, cancellationToken);
            return Results.Ok(response);
        }
    }
}
