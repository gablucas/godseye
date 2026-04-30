using GodsEye.API.DTO;
using GodsEye.API.Interfaces;

namespace GodsEye.API.Services.Queries
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
