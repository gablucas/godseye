using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.UpdateCameraConfigDwellTimeMonitoring
{
    public class UpdateCameraConfigDwellTimeMonitoringHandler : IRequestHandler<UpdateCameraConfigDwellTimeMonitoringRequest, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCameraConfigDwellTimeMonitoringHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<int>> Handle(UpdateCameraConfigDwellTimeMonitoringRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_CONFIG_DWELL_TIME_MONITORING_UPDATE(@P_ID, @P_MAX_DWELL_TIME_MINUTES, @P_MAX_NON_IDENTIFICATION_TIME_MINUTES)";

            var parameters = new
            {
                P_ID = request.Id,
                P_MAX_DWELL_TIME_MINUTES = request.MaxDwellTimeMinutes,
                P_MAX_NON_IDENTIFICATION_TIME_MINUTES = request.MaxNonIdentificationTimeMinutes
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            if (result.Erro == 0)
                return ApiResponse<int>.Ok(result.Id);
            else
                return ApiResponse<int>.Fail(500, "Houve um erro ao cadastra o roi da câmera");
        }
    }
}
