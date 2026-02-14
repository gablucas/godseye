using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.DwellTimeMonitoring.Commands.CreateDwellTimeMonitoring
{
    public class CreateDwellTimeMonitoringHandler : IRequestHandler<CreateDwellTimeMonitoringRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IApplicationDbContext _context;

        public CreateDwellTimeMonitoringHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateDwellTimeMonitoringRequest request, CancellationToken cancellationToken)
        {
            const string sql = "CALL SP_DWELL_TIME_MONITORING_CREATE(@P_CAMERA_ID, @P_PERSON_ID, @P_ENTERED_AT)";

            var parameters = new
            {
                P_CAMERA_ID = request.cameraId,
                P_PERSON_ID = request.personId,
                P_ENTERED_AT = request.enteredAt
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            return ApiResponse<ProcedureResult>.Ok(result);
        }   
    }
}
