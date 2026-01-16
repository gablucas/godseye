using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace GodsEye.Infrastructure.Repositories
{
    public class IncidentRecordingLogRepository : IIncidentRecordingLogRepository
    {
        private readonly AppDbContext _context;

        public IncidentRecordingLogRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<ProcedureResult> Create(int cameraId, DateTime incidentTime)
        {
            var pCameraId = new MySqlParameter("@P_CAMERA_ID", cameraId);
            var pIncidentTime = new MySqlParameter("@P_INCIDENT_TIME", incidentTime)
            {
                DbType = System.Data.DbType.DateTime,
            };

            var result = await _context.ProcedureResult
                .FromSqlRaw(
                "CALL SP_INCIDENT_RECORDING_CREATE_LOG(@P_CAMERA_ID, @P_INCIDENT_TIME)",
                pCameraId, pIncidentTime)
                .ToListAsync();

            return result.FirstOrDefault() ?? new ProcedureResult
            {
                Erro = 1,
                Mensagem = "Houve um erro ao cadastrar o log",
                Id = 0
            };
        }

        public async Task<ProcedureResult> Update(int id, int personId)
        {
            var pLogId = new MySqlParameter("@P_LOG_ID", id);
            var pIncidentTime = new MySqlParameter("@P_INCIDENT_TIME", personId);

            var result = await _context.ProcedureResult
                .FromSqlRaw(
                "CALL SP_INCIDENT_RECORDING_UPDATE_LOG(@P_LOG_ID, @P_INCIDENT_TIME)",
                pLogId, pIncidentTime)
                .ToListAsync();

            return result.FirstOrDefault() ?? new ProcedureResult
            {
                Erro = 1,
                Mensagem = "Houve um erro ao atualizar o log",
                Id = 0
            };
        }
    }
}
