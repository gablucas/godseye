using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Text.Json;

namespace GodsEye.Infrastructure.Repositories
{
    public class CameraRepository : ICameraRepository
    {
        private readonly AppDbContext _context;

        public CameraRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }


        public async Task<ProcedureResult> Create(CameraEntity camera, CancellationToken cancellationToken)
        {
            var pName = new MySqlParameter("@P_NAME", camera.Name);
            var pConnection = new MySqlParameter("@P_CONNECTION", camera.Connection);
            var pSectorId = new MySqlParameter("@P_SECTOR_ID", camera.SectorId);

            var FeaturesJSON = JsonSerializer.Serialize(camera.Features);

            var pFeaturesJSON = new MySqlParameter("@P_FEATURES_JSON", MySqlDbType.JSON)
            {
                Value = FeaturesJSON
            };

            var result = await _context.ProcedureResult
                .FromSqlRaw(
                "CALL SP_CAMERA_CREATE(@P_NAME, @P_CONNECTION, @P_SECTOR_ID, @P_FEATURES_JSON)",
                pName, pConnection, pSectorId, pFeaturesJSON)
                .ToListAsync();

            return result.FirstOrDefault() ?? ProcedureResult.Error("Houve um erro ao executar a procedure de cadastro de camera!");
        }

        public async Task<ProcedureResult> Update(CameraEntity camera, CancellationToken cancellationToken)
        {
            var pCameraId = new MySqlParameter("@P_CAMERA_ID", camera.Id);
            var pName = new MySqlParameter("@P_NAME", camera.Name);
            var pConnection = new MySqlParameter("@P_CONNECTION", camera.Connection);
            var pSectorId = new MySqlParameter("@P_SECTOR_ID", camera.SectorId);

            var FeaturesJSON = JsonSerializer.Serialize(camera.Features);

            var pFeaturesJSON = new MySqlParameter("@P_FEATURES_JSON", MySqlDbType.JSON)
            {
                Value = FeaturesJSON
            };

            var result = await _context.ProcedureResult
                .FromSqlRaw(
                "CALL SP_CAMERA_UPDATE(@P_CAMERA_ID, @P_NAME, @P_CONNECTION, @P_SECTOR_ID, @P_FEATURES_JSON)",
                pCameraId, pName, pConnection, pSectorId, pFeaturesJSON)
                .ToListAsync();

            return result.FirstOrDefault() ?? ProcedureResult.Error("Houve um erro ao executar a procedure de cadastro de camera!");
        }
    }
}
