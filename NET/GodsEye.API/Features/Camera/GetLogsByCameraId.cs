using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Sectors
{
    public sealed record GetLogsByCameraIdCommand(int id) : IRequest<CameraLogResponse>;

    internal sealed class GetLogsByCameraIdHandler(IDapperContext context) : IRequestHandler<GetLogsByCameraIdCommand, CameraLogResponse>
    {
        public async Task<CameraLogResponse> Handle(GetLogsByCameraIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetLogsByCameraIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }

        public async Task<CameraLogResponse?> GetLogsByCameraIdQuery(int cameraId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_ENVIRONMENT_GET_MONITORING_LOG(@P_CAMERA_ID)";

            var parameters = new
            {
                P_CAMERA_ID = cameraId
            };

            return await context.QuerySingleSqlAsync<CameraLogResponse>(sql, parameters, cancellationToken);
        }
    }

    public class GetLogsByCameraIdEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/camera/logs/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetCameraByIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
