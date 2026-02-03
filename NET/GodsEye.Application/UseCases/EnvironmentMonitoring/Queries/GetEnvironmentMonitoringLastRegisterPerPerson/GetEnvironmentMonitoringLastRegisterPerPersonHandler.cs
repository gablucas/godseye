using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetEnvironmentMonitoringPersonsLastSector
{
    public class GetEnvironmentMonitoringLastRegisterPerPersonHandler : IRequestHandler<GetEnvironmentMonitoringLastRegisterPerPerson, ApiResponse<IEnumerable<EnvironmentMonitoringModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetEnvironmentMonitoringLastRegisterPerPersonHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>> Handle(GetEnvironmentMonitoringLastRegisterPerPerson request, CancellationToken cancellationToken)
        {
            var query = "CALL SP_ENVIRONMENT_MONITORING_GET_LAST_REGISTER_PER_PERSON()";

            var result = await _context.QuerySqlAsync<EnvironmentMonitoringModel>(query, cancellationToken);

            return ApiResponse<IEnumerable<EnvironmentMonitoringModel>>.Ok(result);
        }
    }
}
