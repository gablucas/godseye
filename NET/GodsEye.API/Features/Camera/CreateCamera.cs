using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GodsEye.API.Features.Camera
{
    public sealed record CreateCameraRequest(string name, string? connection, int sectorId);

    public class CreateCameraValidator : AbstractValidator<CreateCameraRequest>
    {
        public CreateCameraValidator()
        {
            RuleFor(camera => camera.name).NotEmpty();
            RuleFor(camera => camera.sectorId).NotEmpty();
        }
    }

    internal sealed record CreateCameraCommand(string name, string? connection, int sectorId) : IRequest<int>;

    internal sealed class CreateCameraMapper : Profile
    {
        public CreateCameraMapper()
        {
            CreateMap<CreateCameraRequest, CreateCameraCommand>();
        }
    }

    internal sealed class CreateCameraHandler(IDapperContext context, ILogger<CreateCameraHandler> logger) : IRequestHandler<CreateCameraCommand, int>
    {
        public async Task<int> Handle(CreateCameraCommand request, CancellationToken cancellationToken)
        {
            var result = await CreateCameraWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResponse?> CreateCameraWrite(CreateCameraCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_CREATE(@P_NAME, @P_CONNECTION, @P_SECTOR_ID)";

            var parameters = new
            {
                P_NAME = request.name,
                P_CONNECTION = request.connection,
                P_SECTOR_ID = request.sectorId

            };

            return await context.QuerySingleSqlAsync<ProcedureResponse>(sql, parameters, cancellationToken);
        }
    }

    public class CameraEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/camera", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] CreateCameraRequest request, 
            [FromServices] IValidator<CreateCameraRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = mapper.Map<CreateCameraCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
