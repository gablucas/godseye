using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.GodsEye.Queries.GetMonitoringData
{
    public class GetMonitoringDataHandler : IRequestHandler<GetMonitoringDataRequest, ApiResponse<MonitoringDataModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetMonitoringDataHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<MonitoringDataModel>> Handle(GetMonitoringDataRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_GODSEYE_GET_MONITORING_DATA()";

            var parameters = new { };

            var result = await _context.QuerySingleSqlAsync<MonitoringDataModel>(sql, parameters, cancellationToken);

            return ApiResponse<MonitoringDataModel>.Ok(result);
        }
    }
}
