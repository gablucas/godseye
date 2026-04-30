using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Camera
{
    public sealed record GetAllCamerasCommand() : IRequest<IEnumerable<CameraResponse>>;

    internal sealed record GetAllCamerasHandler(IDapperContext context) : IRequestHandler<GetAllCamerasCommand, IEnumerable<CameraResponse>>
    {
        public async Task<IEnumerable<CameraResponse>> Handle(GetAllCamerasCommand request, CancellationToken cancellationToken)
        {
            return await GetAllCamerasQuery(cancellationToken);
        }

        public async Task<IEnumerable<CameraResponse>> GetAllCamerasQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_GET_ALL()";

            return await context.QuerySqlAsync<CameraResponse>(sql, cancellationToken);
        }
    }

    public class GetAllCamerasEndpoit : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/camera", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetAllCamerasCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
