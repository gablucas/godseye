
using GodsEye.API.DTO;
using GodsEye.API.Interfaces;

namespace GodsEye.API.Services.Queries
{
    public class AccessLevelQuerie : IAccessLevelQuerie
    {
        private readonly IDapperContext _context;

        public AccessLevelQuerie(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccessLevelCache>> GetAllCache(CancellationToken cancellationToken)
        {
            var query = "CALL SP_ACCESS_LEVEL_GET_ALL_CACHE()";

            var parameters = new { };

            return await _context.QuerySqlAsync<AccessLevelCache>(query, parameters, cancellationToken);
        }
    }
}
