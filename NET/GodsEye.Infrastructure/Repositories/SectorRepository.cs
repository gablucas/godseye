using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Text.Json;

namespace GodsEye.Infrastructure.Repositories
{
    public class SectorRepository : ISectorRepository
    {
        private readonly AppDbContext _context;

        public SectorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProcedureResult> Create(SectorEntity sector, CancellationToken cancellationToken)
        {
            var pName = new MySqlParameter("@P_NAME", sector.Name);


            var result = await _context.ProcedureResult
                .FromSqlRaw(
                "CALL SP_SECTOR_CREATE(@P_NAME)",
                pName)
                .ToListAsync();

            return result.FirstOrDefault() ?? ProcedureResult.Error("Houve um erro ao executar a procedure de cadastro de setor!");
        }
    }
}
