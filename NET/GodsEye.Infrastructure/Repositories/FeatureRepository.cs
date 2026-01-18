using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using GodsEye.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GodsEye.Infrastructure.Repositories
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly AppDbContext _context;

        public FeatureRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<FeatureEntity>> GetAll()
        {
            var result = await _context.Feature
                .FromSqlRaw("CALL SP_FEATURE_GET_ALL()")
                .ToListAsync();

            return result;
        }
    }
}
