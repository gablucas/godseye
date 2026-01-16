using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.GodsEye.Queries.GetMonitoringData
{
    public class GetMonitoringDataHandler : IRequestHandler<GetMonitoringDataRequest, ApiResponse<MonitoringDataModel>>
    {
        private readonly IGodsEyeQueryRepository _godsEyeQueryRepository;

        public GetMonitoringDataHandler(IGodsEyeQueryRepository godsEyeQueryRepository)
        {
            _godsEyeQueryRepository = godsEyeQueryRepository;
        }

        public async Task<ApiResponse<MonitoringDataModel>> Handle(GetMonitoringDataRequest request, CancellationToken cancellationToken)
        {
            var result = await _godsEyeQueryRepository.GetMonitoringData();
            return ApiResponse<MonitoringDataModel>.Ok(result);
        }
    }
}
