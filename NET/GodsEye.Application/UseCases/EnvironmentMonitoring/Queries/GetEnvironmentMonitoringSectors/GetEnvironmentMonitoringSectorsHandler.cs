using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetEnvironmentMonitoringSectors
{
    public class GetEnvironmentMonitoringSectorsHandler : IRequestHandler<GetEnvironmentMonitoringSectorsRequest, ApiResponse<IEnumerable<EnvironmentMonitoringSectorModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetEnvironmentMonitoringSectorsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<EnvironmentMonitoringSectorModel>>> Handle(GetEnvironmentMonitoringSectorsRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ENVIRONMENT_MONITORING_GET_SECTORS()";
            var result = await _context.QuerySqlAsync<EnvironmentMonitoringSectorModel>(sql, cancellationToken);
            return ApiResponse<IEnumerable<EnvironmentMonitoringSectorModel>>.Ok(result);
        }
    }
}
