using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetAllEnvironmentMonitoringLogs
{
    public class GetAllEnvironmentMonitoringLogsHandler : IRequestHandler<GetAllEnvironmentMonitoringLogsRequest, ApiResponse<IEnumerable<EnvironmentMonitoringModel>>>
    {
        private readonly IEnvironmentMonitoringQueryRepository _environmentMonitoringQueryRepository;

        public GetAllEnvironmentMonitoringLogsHandler(IEnvironmentMonitoringQueryRepository environmentMonitoringQueryRepository)
        {
            _environmentMonitoringQueryRepository = environmentMonitoringQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>> Handle(GetAllEnvironmentMonitoringLogsRequest request, CancellationToken cancellationToken)
        {
            var result = await _environmentMonitoringQueryRepository.GetAll(cancellationToken);

            return ApiResponse<IEnumerable<EnvironmentMonitoringModel>>.Ok(result);
        }
    }
}
