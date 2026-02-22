using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.AccessSchedule.Queries.GetAccessSchedulesById
{
    public class GetAccessSchedulesByIdHandler : IRequestHandler<GetAccessSchedulesByIdRequest, ApiResponse<AccessScheduleModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetAccessSchedulesByIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<AccessScheduleModel>> Handle(GetAccessSchedulesByIdRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_SCHEDULE_GET_BY_ID(@P_ACCESS_SCHEDULE_ID)";

            var parameters = new
            {
                P_ACCESS_SCHEDULE_ID = request.AccessScheduleId,
            };

            var result = await _context.QuerySingleSqlAsync<AccessScheduleModel>(sql, parameters, cancellationToken);

            return ApiResponse<AccessScheduleModel>.Ok(result);
        }
    }
}
