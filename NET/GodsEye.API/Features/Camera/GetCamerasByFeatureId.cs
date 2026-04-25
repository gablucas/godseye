using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Sectors
{
    public sealed record GetCameraByFeatureIdCommand(int id) : IRequest<CameraResponse>;

    internal sealed class GetCameraByFeatureIdHandler(IDapperContext context) : IRequestHandler<GetCameraByFeatureIdCommand, CameraResponse>
    {
        public async Task<CameraResponse> Handle(GetCameraByFeatureIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetCameraByFeatureIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }

        public async Task<CameraResponse?> GetCameraByFeatureIdQuery(int featureId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_GET_BY_FEATURE_ID(@P_FEATURE_ID)";

            var parameters = new
            {
                P_FEATURE_ID = featureId
            };

            return await context.QuerySingleSqlAsync<CameraResponse>(sql, parameters, cancellationToken);
        }
    }

    public class GetCameraByFeatureIdEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/camera/feature/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetCameraByFeatureIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
