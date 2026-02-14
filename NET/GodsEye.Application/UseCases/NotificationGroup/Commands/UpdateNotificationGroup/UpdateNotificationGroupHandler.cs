using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;
using System.Text.Json;

namespace GodsEye.Application.UseCases.NotificationGroup.Commands.UpdateNotificationGroup
{
    public class UpdateNotificationGroupHandler : IRequestHandler<UpdateNotificationGroupRequest, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateNotificationGroupHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<int>> Handle(UpdateNotificationGroupRequest request, CancellationToken cancellationToken)
        {
            var query = "CALL SP_NOTIFICATION_GROUP_UPDATE(@P_NOTIFICATION_GROUP_ID, @P_NEW_EMAILS_JSON, @P_DELETE_EMAILS_JSON)";

            var pNewEmailsJson = JsonSerializer.Serialize(request.NewEmails);
            var pRemoveEmailsJson = JsonSerializer.Serialize(request.RemoveEmails);

            var parameters = new
            {
                P_NOTIFICATION_GROUP_ID = request.Id,
                P_NEW_EMAILS_JSON = pNewEmailsJson,
                P_DELETE_EMAILS_JSON = pRemoveEmailsJson,
            };

            var result = await _context.ExecuteSqlAsync(query, parameters, cancellationToken);
            return ApiResponse<int>.Ok(result);
        }
    }
}
