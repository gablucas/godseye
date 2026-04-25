using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Sectors
{
    public sealed record GetCameraByIdCommand(int id) : IRequest<CameraResponse>;

    internal sealed class GetCameraByIdHandler(IDapperContext context) : IRequestHandler<GetCameraByIdCommand, CameraResponse>
    {
        public async Task<CameraResponse> Handle(GetCameraByIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetCameraByIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }

        public async Task<CameraResponse?> GetCameraByIdQuery(int cameraId, CancellationToken cancellationToken)
        {
            const string sql = "CALL SP_CAMERA_GET_BY_ID(@P_CAMERA_ID)";

            var parameters = new
            {
                P_CAMERA_ID = cameraId
            };

            return await context.QuerySingleSqlAsync<CameraResponse>(sql, parameters, cancellationToken);
        }
    }

    public class GetCameraByIdEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/camera/{id}", Handle);
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
