using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodsEye.Infrastructure.QuerieRepositories
{
    public class SectorQueryRepository : ISectorQueryRepository
    {
        private readonly AppDbContext _context;

        public SectorQueryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SectorModel>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _context.SectorModel
                .FromSqlRaw("CALL SP_SECTOR_GET_ALL()")
                .ToListAsync();

            return result;
        }
    }
}
