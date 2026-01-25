using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace GodsEye.Infrastructure.QuerieRepositories
{
    public class IncidentRecordingQueryRepository : IIncidentRecordingQueryRepository
    {
        private readonly AppDbContext _context;

        public IncidentRecordingQueryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IncidentRecordingModel>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _context.IncidentRecordingModel
            .FromSqlRaw("CALL SP_INCIDENT_RECORDING_GET_ALL_LOGS()")
            .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<IncidentRecordingModel> GetByLogId(int logId, CancellationToken cancellationToken)
        {
            var pLogId = new MySqlParameter("@P_ID", logId);

            var result = await _context.IncidentRecordingModel
                .FromSqlRaw("CALL SP_INCIDENT_RECORDING_GET_LOG_BY_ID(@P_ID)", pLogId)
                .ToListAsync(cancellationToken);

            return result.FirstOrDefault() ?? new IncidentRecordingModel();
        }

        public async Task<IncidentRecordingProcessModel?> GetToProcess(CancellationToken cancellationToken)
        {
            var result = await _context.IncidentRecordingProcessModel
                .FromSqlRaw("CALL SP_INCIDENT_RECORDING_GET_TO_PROCESS()")
                .ToListAsync(cancellationToken);

            return result.FirstOrDefault();
        }
    }
}
