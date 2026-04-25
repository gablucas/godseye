using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.NotificationGroups;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Features.Camera
{
    public sealed record GetAllNotificationGroupsCommand() : IRequest<IEnumerable<NotificationGroupsResponse>>;

    internal sealed record GetAllNotificationGroupsHandler(IDapperContext context) : IRequestHandler<GetAllNotificationGroupsCommand, IEnumerable<NotificationGroupsResponse>>
    {
        public async Task<IEnumerable<NotificationGroupsResponse>> Handle(GetAllNotificationGroupsCommand request, CancellationToken cancellationToken)
        {
            return await GetAllNotificationGroupsQuery(cancellationToken);
        }

        public async Task<IEnumerable<NotificationGroupsResponse>> GetAllNotificationGroupsQuery(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_NOTIFICATION_GROUP_GET_ALL()";

            return await context.QuerySqlAsync<NotificationGroupsResponse>(sql, cancellationToken);
        }
    }

    public class GetAllNotificationGroupsEndpoint : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/notification-group", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken
            )
        {
            var response = await mediator.Send(new GetAllNotificationGroupsCommand(), cancellationToken);
            return Results.Ok(response);
        }
    }
}
