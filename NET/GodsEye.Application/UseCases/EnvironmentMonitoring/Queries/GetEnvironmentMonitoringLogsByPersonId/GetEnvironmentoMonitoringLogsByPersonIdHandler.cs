using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetEnvironmentMonitoringLogsByPersonId
{
    public class GetEnvironmentoMonitoringLogsByPersonIdHandler : IRequestHandler<GetEnvironmentMonitoringLogsByPersonIdRequest, ApiResponse<EnvironmentMonitoringPersonModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetEnvironmentoMonitoringLogsByPersonIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<EnvironmentMonitoringPersonModel>> Handle(GetEnvironmentMonitoringLogsByPersonIdRequest request, CancellationToken cancellationToken)
        {
            var query = "CALL SP_ENVIRONMENT_MONITORING_GET_LOG_BY_PERSON_ID(@P_PERSON_ID)";

            var parameters = new
            {
                P_PERSON_ID = request.personId,
            };

            var result = await _context.QuerySingleSqlAsync<EnvironmentMonitoringPersonModel>(query, parameters, cancellationToken);
            return ApiResponse<EnvironmentMonitoringPersonModel>.Ok(result);
        }
    }
}
