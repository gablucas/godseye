using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Camera.Roi
{
    public sealed record GetCameraRoiCommand(int id) : IRequest<CameraRoiResponse>;

    internal sealed class GetCameraRoiHandler(IDapperContext context) : IRequestHandler<GetCameraRoiCommand, CameraRoiResponse>
    {
        public async Task<CameraRoiResponse> Handle(GetCameraRoiCommand request, CancellationToken cancellationToken)
        {
            var result = await GetCameraRoiQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }

        public async Task<CameraRoiResponse?> GetCameraRoiQuery(int cameraId, CancellationToken cancellationToken)
        {
            var query = "CALL SP_CAMERA_ROI_GET_BY_CAMERA_ID(@P_CAMERA_ID)";

            var parameters = new
            {
                P_CAMERA_ID = cameraId
            };

            return await context.QuerySingleSqlAsync<CameraRoiResponse>(query, parameters, cancellationToken);
        }
    }

    public class GetCameraRoiEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/camera/roi/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetCameraRoiCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
