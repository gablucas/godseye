using GodsEye.Application.DTOs.Model;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Domain.ValueObjects;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Text.Json;

namespace GodsEye.Infrastructure.Repositories
{
    public class IncidentRecordingLogRepository : IIncidentRecordingLogRepository
    {
        private readonly AppDbContext _context;

        public IncidentRecordingLogRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<ProcedureResult> Create(string macAddress, DateTime incidentTime)
        {
            var pMacAddress = new MySqlParameter("@P_MAC_ADDRESS", macAddress);
            var pIncidentTime = new MySqlParameter("@P_INCIDENT_TIME", incidentTime)
            {
                DbType = System.Data.DbType.DateTime,
            };

            var result = await _context.ProcedureResult
                .FromSqlRaw(
                "CALL SP_INCIDENT_RECORDING_CREATE_LOG(@P_MAC_ADDRESS, @P_INCIDENT_TIME)",
                pMacAddress, pIncidentTime)
                .ToListAsync();

            return result.FirstOrDefault() ?? new ProcedureResult
            {
                Erro = 1,
                Mensagem = "Houve um erro ao cadastrar o log",
                Id = 0
            };
        }

        public async Task<ProcedureResult> Update(int id, List<IncidentRecordingPersonVO> persons, string videoPath)
        {
            var pLogId = new MySqlParameter("@P_LOG_ID", id);

            var personsJSON = JsonSerializer.Serialize(persons);

            var pPersons = new MySqlParameter("@P_PERSONS_JSON", MySqlDbType.JSON)
            {
                Value = personsJSON
            };

            var pVideoPath = new MySqlParameter("@P_VIDEO_PATH", videoPath);

            var result = await _context.ProcedureResult
                .FromSqlRaw(
                "CALL SP_INCIDENT_RECORDING_UPDATE_LOG(@P_LOG_ID, @P_PERSONS_JSON, @P_VIDEO_PATH)",
                pLogId, pPersons, pVideoPath)
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
