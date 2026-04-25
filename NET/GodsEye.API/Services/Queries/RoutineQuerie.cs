using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;

namespace GodsEye.Infrastructure.Queries
{
    public class RoutineQuerie : IRoutineQuerie
    {
        private readonly IDapperContext _context;

        public RoutineQuerie(IDapperContext context)
        {
            _context = context;
        }

        public async Task<RoutineModel?> GetById(int routineId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ROUTINE_GET_BY_ID(@P_ROUTINE_ID)";
            return await _context.QuerySingleSqlAsync<RoutineModel>(sql, new { @P_ROUTINE_ID = routineId }, cancellationToken);
        }
    }
}
