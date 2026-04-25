using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Queries;
namespace GodsEye.Infrastructure.Queries
{
    public class ComplianceQuerie : IComplianceQuerie
    {
        private readonly IDapperContext _context;

        public ComplianceQuerie(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CompliancePolicyDTO>> GetAll(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_GET_ALL()";

            return await _context.QuerySqlAsync<CompliancePolicyDTO>(sql, cancellationToken);
        }
        public async Task<CompliancePolicyDTO?> GeById(int complianceId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_GET_BY_ID(@P_COMPLIANCE_ID)";

            var parameters = new { P_COMPLIANCE_ID = complianceId } ;

            return await _context.QuerySingleSqlAsync<CompliancePolicyDTO>(sql, parameters, cancellationToken);
        }
    }
}
