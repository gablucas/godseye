using AutoMapper;
using GodsEye.API.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace GodsEye.API.Features.Camera
{
    public sealed record UpdateCameraRequest(int id, string name, string? connection, int sectorId, IEnumerable<int> features);

    internal sealed record UpdateCameraCommand(int id, string name, string? connection, int sectorId, IEnumerable<int> features) : IRequest<int>;
    
    internal sealed class UpdateCameraMapper : Profile
    {
        public UpdateCameraMapper()
        {
            CreateMap<UpdateCameraRequest, UpdateCameraCommand>();
        }
    }

    internal sealed class UpdateCameraHandler(IDapperContext context, ILogger<UpdateCameraHandler> logger) : IRequestHandler<UpdateCameraCommand, int>
    {
        public async Task<int> Handle(UpdateCameraCommand request, CancellationToken cancellationToken)
        {
            var result = await UpdateCameraWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResult?> UpdateCameraWrite(UpdateCameraCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_UPDATE(@P_CAMERA_ID, @P_NAME, @P_CONNECTION, @P_SECTOR_ID, @P_FEATURES_JSON)";

            var parameters = new
            {
                P_CAMERA_ID = request.id,
                P_NAME = request.name,
                P_CONNECTION = request.connection,
                P_SECTOR_ID = request.sectorId,
                P_FEATURES_JSON = JsonSerializer.Serialize(request.features)

            };

            return await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }
    }

    public class UpdateCameraEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPut("/api/camera", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] UpdateCameraRequest request,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var command = mapper.Map<UpdateCameraCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
