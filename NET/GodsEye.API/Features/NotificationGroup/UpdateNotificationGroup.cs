using AutoMapper;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace GodsEye.API.Features.Camera
{
    public sealed record UpdateNotificationGroupRequest(int Id, List<string> NewEmails, List<int> RemoveEmails);

    internal sealed record UpdateNotificationGroupCommand(int Id, List<string> NewEmails, List<int> RemoveEmails) : IRequest<int>;
    
    internal sealed class UpdateNotificationGroupMapper : Profile
    {
        public UpdateNotificationGroupMapper()
        {
            CreateMap<UpdateNotificationGroupRequest, UpdateNotificationGroupCommand>();
        }
    }

    internal sealed class UpdateNotificationGroupHandler(IDapperContext context, ILogger<UpdateNotificationGroupHandler> logger) : IRequestHandler<UpdateNotificationGroupCommand, int>
    {
        public async Task<int> Handle(UpdateNotificationGroupCommand request, CancellationToken cancellationToken)
        {
            var result = await UpdateNotificationGroupWrite(request, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao criar o setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            return result.Id;
        }

        public async Task<ProcedureResponse?> UpdateNotificationGroupWrite(UpdateNotificationGroupCommand request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_NOTIFICATION_GROUP_UPDATE(@P_NOTIFICATION_GROUP_ID, @P_NEW_EMAILS_JSON, @P_DELETE_EMAILS_JSON)";

            var pNewEmailsJson = JsonSerializer.Serialize(request.NewEmails);
            var pRemoveEmailsJson = JsonSerializer.Serialize(request.RemoveEmails);

            var parameters = new
            {
                P_NOTIFICATION_GROUP_ID = request.Id,
                P_NEW_EMAILS_JSON = pNewEmailsJson,
                P_DELETE_EMAILS_JSON = pRemoveEmailsJson,
            };

            return await context.QuerySingleSqlAsync<ProcedureResponse>(sql, parameters, cancellationToken);
        }
    }

    public class UpdateNotificationGroupEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapPut("/api/notification-group", Handle);
        }

        private static async Task<IResult> Handle(
            [FromBody] UpdateNotificationGroupRequest request,
            [FromServices] IMapper mapper,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var command = mapper.Map<UpdateNotificationGroupCommand>(request);
            var response = await mediator.Send(command, cancellationToken);

            return Results.Ok(response);
        }
    }
}
