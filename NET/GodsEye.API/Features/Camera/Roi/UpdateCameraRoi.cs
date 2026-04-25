using AutoMapper;
using GodsEye.API.Interfaces;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GodsEye.API.Features.Camera
{
    public sealed record UpdateCameraRoiRequest(int roiId, RoiDTO coordinates, bool isActive);

    internal sealed record UpdateCameraRoiCommand(int roiId, RoiDTO coordinates, bool isActive) : IRequest<int>;
    
    internal sealed class UpdateCameraRoiMapper : Profile
    {
        public UpdateCameraRoiMapper()
        {
            CreateMap<UpdateCameraRoiRequest, UpdateCameraRoiCommand>();
        }
    }

    internal sealed class UpdateCameraRoiHandler(IDapperContext context, ILogger<UpdateCameraRoiHandler> logger) : IRequestHandler<UpdateCameraRoiCommand, int>
    {
        public async Task<int> Handle(UpdateCameraRoiCommand request, CancellationToken cancellationToken)
        {
            var result = await UpdateCameraRoiWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResult?> UpdateCameraRoiWrite(UpdateCameraRoiCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_ROI_UPDATE(@P_CAMERA_ROI_ID, @P_COORDINATES_JSON, @P_IS_ACTIVE)";

            var parameters = new
            {
                P_CAMERA_ROI_ID = request.roiId,
                P_COORDINATES_JSON = JsonSerializer.Serialize(request.coordinates),
                P_IS_ACTIVE = request.isActive
            };

            return await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }
    }

    public class UpdateCameraRoiEnpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPut("/api/camera/roi", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] UpdateCameraRoiRequest request,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var command = mapper.Map<UpdateCameraRoiCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
