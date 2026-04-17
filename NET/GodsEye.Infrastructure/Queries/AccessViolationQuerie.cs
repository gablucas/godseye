using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;

namespace GodsEye.Infrastructure.Queries
{
    public class AccessViolationQuerie : IAccessViolationQuerie
    {
        private readonly IDapperContext _context;

        public AccessViolationQuerie(IDapperContext context)
        {
            _context = context;
        }

        public async Task<AccessViolationDetailDTO?> GetAccessViolationDetail(int personId, int sectorId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_VIOLATION_GET_DETAIL(@P_PERSON_ID, @P_SECTOR_ID)";

            var parameters = new
            {
                P_PERSON_ID = personId,
                P_SECTOR_ID = sectorId
            };

            return await _context.QuerySingleSqlAsync<AccessViolationDetailDTO>(sql, parameters, cancellationToken);
        }
    }
}
