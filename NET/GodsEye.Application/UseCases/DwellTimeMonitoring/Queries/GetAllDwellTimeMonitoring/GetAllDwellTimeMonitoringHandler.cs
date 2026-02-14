using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.DwellTimeMonitoring.Queries.GetAllDwellTimeMonitoring
{
    public class GetAllDwellTimeMonitoringHandler : IRequestHandler<GetAllDwellTimeMonitoringRequest, IEnumerable<DwellTimeMonitoringModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllDwellTimeMonitoringHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DwellTimeMonitoringModel>> Handle(GetAllDwellTimeMonitoringRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_DWELL_TIME_MONITORING_GET_ALL()";

            var dwellTimeMonitoring = await _context.QuerySqlAsync<DwellTimeMonitoringModel>(sql, cancellationToken);

            return dwellTimeMonitoring;
        }
    }
}
