using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetAllEnvironmentMonitoringLogs
{
    public class GetAllEnvironmentMonitoringLogsHandler : IRequestHandler<GetAllEnvironmentMonitoringLogsRequest, ApiResponse<IEnumerable<EnvironmentMonitoringModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllEnvironmentMonitoringLogsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>> Handle(GetAllEnvironmentMonitoringLogsRequest request, CancellationToken cancellationToken)
        {
            var query = "CALL SP_ENVIRONMENT_MONITORING_GET_ALL_LOGS(@P_PAGE_NUMBER, @P_PAGE_SIZE)";

            var parameters = new
            {
                P_PAGE_NUMBER = request.pageNumber,
                P_PAGE_SIZE = request.pageSize,
            };

            var result = await _context.QuerySqlAsync<EnvironmentMonitoringModel>(query, parameters, cancellationToken);
            return ApiResponse<IEnumerable<EnvironmentMonitoringModel>>.Ok(result);
        }
    }
}
