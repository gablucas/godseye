
using GodsEye.API.DTO;
using GodsEye.API.Interfaces;
using GodsEye.API.Services.Queries;

namespace GodsEye.API.Queries
{
    public class AccessViolationQuerie : IAccessViolationQuerie
    {
        private readonly IDapperContext _context;

        public AccessViolationQuerie(IDapperContext context)
        {
            _context = context;
        }

        public async Task<AccessViolationDetailResponse?> GetAccessViolationDetail(int personId, int sectorId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ACCESS_VIOLATION_GET_DETAIL(@P_PERSON_ID, @P_SECTOR_ID)";

            var parameters = new
            {
                P_PERSON_ID = personId,
                P_SECTOR_ID = sectorId
            };

            return await _context.QuerySingleSqlAsync<AccessViolationDetailResponse>(sql, parameters, cancellationToken);
        }
    }
}
