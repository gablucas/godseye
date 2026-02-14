using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.NotificationGroup.Queries.GetAllNotificationGroups
{
    public class GetAllNotificationGroupsHandler : IRequestHandler<GetAllNotificationGroupsRequest, ApiResponse<IEnumerable<NotificationGroupModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllNotificationGroupsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<NotificationGroupModel>>> Handle(GetAllNotificationGroupsRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_NOTIFICATION_GROUP_GET_ALL()";

            var parameters = new { };

            var result = await _context.QuerySqlAsync<NotificationGroupModel>(sql, parameters, cancellationToken);
            return ApiResponse<IEnumerable<NotificationGroupModel>>.Ok(result);
        }
    }
}
