using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace GodsEye.Infrastructure.Repositories
{
    public class DwellTimeMonitoringRepository : IDwellTimeMonitoringRepository
    {
        private readonly AppDbContext _context;

        public DwellTimeMonitoringRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProcedureResult> Create(DwellTimeMonitoringEntity dwellTimeMonitoring, CancellationToken cancellationToken)
        {
            var pCameraId = new MySqlParameter("@P_CAMERA_ID", dwellTimeMonitoring.CameraId);
            var pPersonId = new MySqlParameter("@P_PERSON_ID", dwellTimeMonitoring.PersonId);

            var pEnteredAt = new MySqlParameter("@P_ENTERED_AT", dwellTimeMonitoring.EnteredAt)
            {
                DbType = System.Data.DbType.DateTime,
            };

            var result = await _context.ProcedureResult.
                FromSqlRaw("CALL SP_DWELL_TIME_MONITORING_CREATE(@P_CAMERA_ID, @P_PERSON_ID, @P_ENTERED_AT)", pCameraId, pPersonId, pEnteredAt)
                .ToListAsync(cancellationToken);

            return result.FirstOrDefault() ?? ProcedureResult.Error();
        }

        public async Task<ProcedureResult> Update(DwellTimeMonitoringEntity dwellTimeMonitoring, CancellationToken cancellationToken)
        {
            var pId = new MySqlParameter("@P_ID", dwellTimeMonitoring.Id);

            var result = await _context.ProcedureResult.
                FromSqlRaw("CALL SP_DWELL_TIME_MONITORING_UPDATE(@P_ID)", pId)
                .ToListAsync(cancellationToken);

            return result.FirstOrDefault() ?? ProcedureResult.Error();
        }
    }
}
