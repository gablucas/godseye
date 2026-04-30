using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;

using GodsEye.Shared.Enums;
using GodsEye.Shared.Response;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GodsEye.API.Features.Camera.Roi
{
    public sealed record CreateCameraRoiRequest(int cameraId, RoiTypeEnum roiType, RoiDTO Coordinates);

    public class CreateCameraRoiValidator : AbstractValidator<CreateCameraRoiRequest>
    {
        public CreateCameraRoiValidator()
        {
            RuleFor(camera => camera.cameraId).NotEmpty();
            RuleFor(camera => camera.roiType).NotEmpty();
            RuleFor(camera => camera.Coordinates).NotEmpty();
        }
    }

    internal sealed record CreateCameraRoiCommand(int cameraId, RoiTypeEnum roiType, RoiDTO Coordinates) : IRequest<int>;

    internal sealed class CreateCameraRoiMapper : Profile
    {
        public CreateCameraRoiMapper()
        {
            CreateMap<CreateCameraRoiRequest, CreateCameraRoiCommand>();
        }
    }

    internal sealed class CreateCameraRoiHandler(IDapperContext context, ILogger<CreateCameraRoiHandler> logger) : IRequestHandler<CreateCameraRoiCommand, int>
    {
        public async Task<int> Handle(CreateCameraRoiCommand request, CancellationToken cancellationToken)
        {
            var result = await CreateCameraRoiWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o ROI da camera";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResponse?> CreateCameraRoiWrite(CreateCameraRoiCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_ROI_CREATE(@P_CAMERA_ID, @P_ROI_TYPE, @P_COORDINATES_JSON)";

            var parameters = new
            {
                P_CAMERA_ID = request.cameraId,
                P_ROI_TYPE = request.roiType,
                P_COORDINATES_JSON = JsonSerializer.Serialize(request.Coordinates),
            };

            return await context.QuerySingleSqlAsync<ProcedureResponse>(sql, parameters, cancellationToken);
        }
    }

    public class CreateCameraRoiEnpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/camera/roi", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] CreateCameraRoiRequest request, 
            [FromServices] IValidator<CreateCameraRoiRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = mapper.Map<CreateCameraRoiCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
