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

            return result.FirstOrDefault() ?? ProcedureResult.Error("Houve um erro ao executar a procedure de registro de incidente!");
        }

        public async Task<ProcedureResult> Update(int id, List<IncidentRecordingPersonVO> persons, string fileName)
        {
            var pLogId = new MySqlParameter("@P_ID", id);

            var personsJSON = JsonSerializer.Serialize(persons);

            var pPersons = new MySqlParameter("@P_PERSONS_JSON", MySqlDbType.JSON)
            {
                Value = personsJSON
            };

            var pFilename = new MySqlParameter("@P_FILE_NAME", fileName);

            var result = await _context.ProcedureResult
                .FromSqlRaw(
                "CALL SP_INCIDENT_RECORDING_UPDATE_LOG(@P_ID, @P_PERSONS_JSON, @P_FILE_NAME)",
                pLogId, pPersons, pFilename)
                .ToListAsync();

            return result.FirstOrDefault() ?? ProcedureResult.Error("Houve um erro ao executar a procedure de atualização de registro de incidente!");
        }
    }
}
