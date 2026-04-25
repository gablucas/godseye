using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GodsEye.API.Features.Camera
{
    public sealed record CreateCameraRequest(string name, string? connection, int sectorId, IEnumerable<int> features);

    public class CreateCameraValidator : AbstractValidator<CreateCameraRequest>
    {
        public CreateCameraValidator()
        {
            RuleFor(camera => camera.name).NotEmpty();
            RuleFor(camera => camera.sectorId).NotEmpty();
            RuleFor(camera => camera.features)
                .NotEmpty()
                .ForEach(rule => rule.GreaterThan(0));
        }
    }

    internal sealed record CreateCameraCommand(string name, string? connection, int sectorId, IEnumerable<int> features) : IRequest<int>;

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

        public async Task<ProcedureResult?> CreateCameraWrite(CreateCameraCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_CREATE(@P_NAME, @P_CONNECTION, @P_SECTOR_ID, @P_FEATURES_JSON)";

            var parameters = new
            {
                P_NAME = request.name,
                P_CONNECTION = request.connection,
                P_SECTOR_ID = request.sectorId,
                P_FEATURES_JSON = JsonSerializer.Serialize(request.features)

            };

            return await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }
    }

    public class CreateCameraEnpoint : IEndpoint
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
