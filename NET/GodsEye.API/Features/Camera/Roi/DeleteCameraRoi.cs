using AutoMapper;
using GodsEye.API.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GodsEye.API.Features.Camera
{
    public sealed record DeleteCameraRoiRequest(int roiId);

    internal sealed record DeleteCameraRoiCommand(int roiId) : IRequest<int>;
    
    internal sealed class DeleteCameraRoiMapper : Profile
    {
        public DeleteCameraRoiMapper()
        {
            CreateMap<DeleteCameraRoiRequest, DeleteCameraRoiCommand>();
        }
    }

    internal sealed class DeleteCameraRoiHandler(IDapperContext context, ILogger<DeleteCameraRoiHandler> logger) : IRequestHandler<DeleteCameraRoiCommand, int>
    {
        public async Task<int> Handle(DeleteCameraRoiCommand request, CancellationToken cancellationToken)
        {
            var result = await DeleteCameraRoiWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResult?> DeleteCameraRoiWrite(DeleteCameraRoiCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_ROI_DELETE(@P_CAMERA_ROI_ID)";

            var parameters = new
            {
                P_CAMERA_ROI_ID = request.roiId,
            };

            return await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }
    }

    public class DeleteCameraRoiEnpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapDelete("/api/camera/roi", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] DeleteCameraRoiRequest request,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var command = mapper.Map<DeleteCameraRoiCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
