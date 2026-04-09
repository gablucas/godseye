using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Write;
using GodsEye.Domain.DTOs.Result;

namespace GodsEye.Infrastructure.Write
{
    public class EnvironmentMonitoringWrite : IEnvironmentMonitoringWrite
    {
        private readonly IDapperContext _context;

        public EnvironmentMonitoringWrite(IDapperContext context)
        {
            _context = context;
        }

        public async Task<ProcedureResult?> Create(int cameraId, int personId, float score, DateTime extractedAt, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ENVIRONMENT_MONITORING_CREATE_LOG(@P_CAMERA_ID, @P_PERSON_ID, @P_SCORE, @P_IDENTIFY_DATE)";

            var parameters = new
            {
                P_CAMERA_ID = cameraId,
                P_PERSON_ID = personId,
                P_SCORE = score,
                P_IDENTIFY_DATE = extractedAt,
            };

            return await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);
        }
    }
}
