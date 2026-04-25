using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Sectors
{
    public sealed record GetNotificationGroupByIdCommand(int id) : IRequest<CameraResponse>;

    internal sealed class GetNotificationGroupByIdHandler(IDapperContext context) : IRequestHandler<GetNotificationGroupByIdCommand, CameraResponse>
    {
        public async Task<CameraResponse> Handle(GetNotificationGroupByIdCommand request, CancellationToken cancellationToken)
        {
            var result = await GetNotificationGroupByIdQuery(request.id, cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o setor solicitado");

            return result;
        }

        public async Task<CameraResponse?> GetNotificationGroupByIdQuery(int notificationGroupId, CancellationToken cancellationToken)
        {
            const string sql = "CALL SP_NOTIFICATION_GROUP_GET_BY_ID(@P_NOTIFICATION_GROUP_ID)";

            var parameters = new
            {
                P_NOTIFICATION_GROUP_ID = notificationGroupId,
            };

            return await context.QuerySingleSqlAsync<CameraResponse>(sql, parameters, cancellationToken);
        }
    }

    public class GetNotificationGroupByIdEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/notification-group/{id}", Handle);
        }

        private static async Task<IResult> Handle(
            [FromRoute] int id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetNotificationGroupByIdCommand(id), cancellationToken);
            return Results.Ok(response);
        }
    }
}
