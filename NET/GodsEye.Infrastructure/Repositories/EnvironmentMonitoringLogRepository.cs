using GodsEye.Application.DTOs.Model;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace GodsEye.Infrastructure.Repositories
{
    public class EnvironmentMonitoringLogRepository : IEnvironmentMonitoringLogRepository
    {
        private readonly AppDbContext _context;

        public EnvironmentMonitoringLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProcedureResult> Create(int cameraId, int personId, decimal score)
        {
            var pCameraId = new MySqlParameter("@P_CAMERA_ID", cameraId);
            var pPersonId = new MySqlParameter("@P_PERSON_ID", personId);
            var pScore = new MySqlParameter("@P_SCORE", score);

            var result = await _context.ProcedureResult
                .FromSqlRaw(
                "CALL SP_ENVIRONMENT_MONITORING_CREATE_LOG(@P_CAMERA_ID, @P_PERSON_ID, @P_SCORE)",
                pCameraId, pPersonId, pScore)
                .ToListAsync();

            return result.FirstOrDefault() ?? new ProcedureResult
            {
                Erro = 1,
                Mensagem = "Houve um erro ao cadastrar o log",
                Id = 0
            };
        }
    }
}
