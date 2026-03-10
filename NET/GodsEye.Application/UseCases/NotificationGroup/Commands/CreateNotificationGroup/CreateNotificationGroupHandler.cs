using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;
using System.Text.Json;

namespace GodsEye.Application.UseCases.NotificationGroup.Commands.CreateNotificationGroup
{
    public class CreateNotificationGroupHandler : IRequestHandler<CreateNotificationGroupRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IApplicationDbContext _context;

        public CreateNotificationGroupHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateNotificationGroupRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_NOTIFICATION_GROUP_CREATE(@P_NAME, @P_EMAILS_JSON)";

            var parameters = new
            {
                P_NAME = request.name,
                P_EMAILS_JSON = JsonSerializer.Serialize(request.emails)
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
