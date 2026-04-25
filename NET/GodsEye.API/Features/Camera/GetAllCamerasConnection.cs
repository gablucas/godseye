using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Camera
{
    public sealed record GetAllCamerasConnectionCommand() : IRequest<IEnumerable<CameraConnectionResponse>>;

    internal sealed record GetAllCamerasConnectionHandler(IDapperContext context) : IRequestHandler<GetAllCamerasConnectionCommand, IEnumerable<CameraConnectionResponse>>
    {
        public async Task<IEnumerable<CameraConnectionResponse>> Handle(GetAllCamerasConnectionCommand request, CancellationToken cancellationToken)
        {
            return await GetAllCamerasConnectionQuery(cancellationToken);
        }

        public async Task<IEnumerable<CameraConnectionResponse>> GetAllCamerasConnectionQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_GET_ALL_CAMERA_CONNECTION()";

            return await context.QuerySqlAsync<CameraConnectionResponse>(sql, cancellationToken);
        }
    }

    public class GetAllCamerasConnectionEnpoit : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/camera/connection", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetAllCamerasConnectionCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
