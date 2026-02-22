using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.AccessSchedule.Queries.GetAllAccessSchedules
{
    public class GetAllAccessSchedulesHandler : IRequestHandler<GetAllAccessSchedulesRequest, ApiResponse<IEnumerable<AccessScheduleModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllAccessSchedulesHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<AccessScheduleModel>>> Handle(GetAllAccessSchedulesRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_SCHEDULE_GET_ALL()";

            var parameters = new { };

            var result = await _context.QuerySqlAsync<AccessScheduleModel>(sql, parameters, cancellationToken);

            return ApiResponse<IEnumerable<AccessScheduleModel>>.Ok(result);
        }
    }
}
