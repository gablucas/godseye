using AutoMapper;
using FluentValidation;
using GodsEye.API.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GodsEye.API.Features.Camera
{
    public sealed record CreateNotificationGroupRequest(string name, IEnumerable<string> emails);

    public class CreateNotificationGroupValidator : AbstractValidator<CreateNotificationGroupRequest>
    {
        public CreateNotificationGroupValidator()
        {
            RuleFor(camera => camera.name).NotEmpty();
            RuleFor(camera => camera.emails).NotEmpty();
        }
    }

    internal sealed record CreateNotificationGroupCommand(string name, IEnumerable<string> emails) : IRequest<int>;

    internal sealed class CreateNotificationGroupMapper : Profile
    {
        public CreateNotificationGroupMapper()
        {
            CreateMap<CreateNotificationGroupRequest, CreateNotificationGroupCommand>();
        }
    }

    internal sealed class CreateNotificationGroupHandler(IDapperContext context, ILogger<CreateNotificationGroupHandler> logger) : IRequestHandler<CreateNotificationGroupCommand, int>
    {
        public async Task<int> Handle(CreateNotificationGroupCommand request, CancellationToken cancellationToken)
        {
            var result = await CreateNotificationGroupWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResult?> CreateNotificationGroupWrite(CreateNotificationGroupCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_NOTIFICATION_GROUP_CREATE(@P_NAME, @P_EMAILS_JSON)";

            var parameters = new
            {
                P_NAME = request.name,
                P_EMAILS_JSON = JsonSerializer.Serialize(request.emails)
            };

            return await context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }
    }

    public class CreateNotificationGroupEnpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPost("/api/notification-group", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] CreateNotificationGroupRequest request, 
            [FromServices] IValidator<CreateNotificationGroupRequest> validator,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator, 
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var command = mapper.Map<CreateNotificationGroupCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
