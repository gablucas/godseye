using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Camera.IncidentRecording
{
    public sealed record GetIncidentRecordingCommand(int cameraId) : IRequest<CameraConfigIncidentRecordingResponse>;

    internal sealed class GetIncidentRecordingHandler(IDapperContext context) : IRequestHandler<GetIncidentRecordingCommand, CameraConfigIncidentRecordingResponse>
    {
        public async Task<CameraConfigIncidentRecordingResponse> Handle(GetIncidentRecordingCommand request, CancellationToken cancellationToken)
        {
            var result = await GetIncidentRecordingQuery(request.cameraId, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar a configuração");

            return result;
        }

        public async Task<CameraConfigIncidentRecordingResponse?> GetIncidentRecordingQuery(int cameraId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_CONFIG_INCIDENT_RECORDING_GET_BY_CAMERA_ID(@P_CAMERA_ID)";

            var parameters = new
            {
                P_CAMERA_ID = cameraId
            };


            return await context.QuerySingleSqlAsync<CameraConfigIncidentRecordingResponse>(sql, parameters, cancellationToken);
        }
    }

    public class GetIncidentRecordingEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/camera/config/incident-recording/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetIncidentRecordingCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
