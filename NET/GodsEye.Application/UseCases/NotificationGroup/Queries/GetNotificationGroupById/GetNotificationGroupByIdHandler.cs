using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.NotificationGroup.Queries.GetNotificationGroupById
{
    public class GetNotificationGroupByIdHandler : IRequestHandler<GetNotificationGroupByIdRequest, ApiResponse<NotificationGroupModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetNotificationGroupByIdHandler(IApplicationDbContext context)
        {   
            _context = context;
        }

        public async Task<ApiResponse<NotificationGroupModel>> Handle(GetNotificationGroupByIdRequest request, CancellationToken cancellationToken)
        {
            var query = "CALL SP_NOTIFICATION_GROUP_GET_BY_ID(@P_NOTIFICATION_GROUP_ID)";

            var parameters = new
            {
                P_NOTIFICATION_GROUP_ID = request.Id,
            };

            var result = await _context.QuerySingleSqlAsync<NotificationGroupModel>(query, parameters, cancellationToken);

            if (result == null)
                return ApiResponse<NotificationGroupModel>.Fail(500, "Não há nenhum dado para esse ID solicitado");

            return ApiResponse<NotificationGroupModel>.Ok(result);
        }
    }
}
