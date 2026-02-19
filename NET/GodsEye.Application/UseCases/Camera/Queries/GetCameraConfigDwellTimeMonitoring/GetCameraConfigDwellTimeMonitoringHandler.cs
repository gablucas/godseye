using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.GetCameraConfigDwellTimeMonitoring
{
    public class GetCameraConfigDwellTimeMonitoringHandler : IRequestHandler<GetCameraConfigDwellTimeMonitoringRequest, ApiResponse<CameraConfigDwellTimeMonitoringModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetCameraConfigDwellTimeMonitoringHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<CameraConfigDwellTimeMonitoringModel>> Handle(GetCameraConfigDwellTimeMonitoringRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_CONFIG_DWELL_TIME_MONITORING_GET_BY_CAMERA_ID(@P_CAMERA_ID)";

            var parameters = new
            {
                P_CAMERA_ID = request.CameraId,
            };

            var result = await _context.QuerySingleSqlAsync<CameraConfigDwellTimeMonitoringModel>(sql, parameters, cancellationToken);

            return ApiResponse<CameraConfigDwellTimeMonitoringModel>.Ok(result);
        }
    }
}
