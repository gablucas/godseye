using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.DwellTimeMonitoring.Queries.GetDwellTimeMonitoringDetailsByCameraId
{
    public class GetDwellTimeMonitoringDetailsByCameraIdHandler : IRequestHandler<GetDwellTimeMonitoringDetailsByCameraIdRequest, ApiResponse<IEnumerable<DwellTimeMonitoringDetailsModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetDwellTimeMonitoringDetailsByCameraIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<DwellTimeMonitoringDetailsModel>>> Handle(GetDwellTimeMonitoringDetailsByCameraIdRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_DWELL_TIME_MONITORING_GET_DETAILS_BY_CAMERA_ID(@P_CAMERA_ID)";

            var parameters = new
            {
                P_CAMERA_ID = request.cameraId,
            };

            var result = await _context.QuerySqlAsync<DwellTimeMonitoringDetailsModel>(sql, parameters, cancellationToken);
            return ApiResponse<IEnumerable<DwellTimeMonitoringDetailsModel>>.Ok(result);
        }
    }
}
