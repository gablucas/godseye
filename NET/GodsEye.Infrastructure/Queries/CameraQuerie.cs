using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;

namespace GodsEye.Infrastructure.Queries
{
    public class CameraQuerie : ICameraQuerie
    {
        private readonly IDapperContext _context;

        public CameraQuerie(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CameraCache>> GetAllCache(CancellationToken cancellationToken)
        {
            var query = "CALL SP_CAMERA_GET_ALL_CACHE()";

            var parameters = new { };

            return await _context.QuerySqlAsync<CameraCache>(query, parameters, cancellationToken);
        }
    }
}
