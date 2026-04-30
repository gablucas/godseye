using AutoMapper;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Camera
{
    public sealed record UpdateCameraIncidentRecordingRequest(int cameraId, string macAddress);

    internal sealed record UpdateCameraIncidentRecordingCommand(int cameraId, string macAddress) : IRequest<int>;
    
    internal sealed class UpdateCameraIncidentRecordingMapper : Profile
    {
        public UpdateCameraIncidentRecordingMapper()
        {
            CreateMap<UpdateCameraIncidentRecordingRequest, UpdateCameraIncidentRecordingCommand>();
        }
    }

    internal sealed class UpdateCameraIncidentRecordingHandler(IDapperContext context, ILogger<UpdateCameraIncidentRecordingHandler> logger) : IRequestHandler<UpdateCameraIncidentRecordingCommand, int>
    {
        public async Task<int> Handle(UpdateCameraIncidentRecordingCommand request, CancellationToken cancellationToken)
        {
            var result = await UpdateCameraIncidentRecordingWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResponse?> UpdateCameraIncidentRecordingWrite(UpdateCameraIncidentRecordingCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_CONFIG_INCIDENT_RECORDING_UPDATE(@P_CAMERA_ID, @P_MAC_ADDRESS)";

            var parameters = new
            {
                P_CAMERA_ID = request.cameraId,
                P_MAC_ADDRESS = request.macAddress,
            };

            return await context.QuerySingleSqlAsync<ProcedureResponse>(sql, parameters, cancellationToken);
        }
    }

    public class UpdateCameraIncidentRecordingEnpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPut("/api/camera/config/incident-recording", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] UpdateCameraIncidentRecordingRequest request,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var command = mapper.Map<UpdateCameraIncidentRecordingCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
