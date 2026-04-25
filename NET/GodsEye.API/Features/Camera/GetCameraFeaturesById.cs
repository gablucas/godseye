using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Sectors
{
    public sealed record GetCameraFeaturesByIdCommand(int id) : IRequest<IEnumerable<CameraFeatureResponse>>;

    internal sealed class GetCameraFeaturesByIdHandler(IDapperContext context) : IRequestHandler<GetCameraFeaturesByIdCommand, IEnumerable<CameraFeatureResponse>>
    {
        public async Task<IEnumerable<CameraFeatureResponse>> Handle(GetCameraFeaturesByIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetCameraByIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }

        public async Task<IEnumerable<CameraFeatureResponse>> GetCameraByIdQuery(int cameraId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_GET_FEATURES_BY_CAMERA_ID(@P_CAMERA_ID)";

            var parameters = new
            {
                P_CAMERA_ID = cameraId
            };

            return await context.QuerySqlAsync<CameraFeatureResponse>(sql, parameters, cancellationToken);
        }
    }

    public class GetCameraFeaturesByIdEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/camera/active-features/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetCameraFeaturesByIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
