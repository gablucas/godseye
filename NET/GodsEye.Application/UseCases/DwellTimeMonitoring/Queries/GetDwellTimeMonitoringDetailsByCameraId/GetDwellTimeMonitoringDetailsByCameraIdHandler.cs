using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.DwellTimeMonitoring.Queries.GetDwellTimeMonitoringDetailsByCameraId
{
    public class GetDwellTimeMonitoringDetailsByCameraIdHandler : IRequestHandler<GetDwellTimeMonitoringDetailsByCameraIdRequest, ApiResponse<IEnumerable<DwellTimeMonitoringDetailsModel>>>
    {
        private readonly IDwellTimeMonitoringQueryRepository _dwellTimeMonitoringQueryRepository;

        public GetDwellTimeMonitoringDetailsByCameraIdHandler(IDwellTimeMonitoringQueryRepository dwellTimeMonitoringQueryRepository)
        {
            _dwellTimeMonitoringQueryRepository = dwellTimeMonitoringQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<DwellTimeMonitoringDetailsModel>>> Handle(GetDwellTimeMonitoringDetailsByCameraIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _dwellTimeMonitoringQueryRepository.GetDetailsByCameraId(request.cameraId, cancellationToken);
            return ApiResponse<IEnumerable<DwellTimeMonitoringDetailsModel>>.Ok(result);
        }
    }
}
