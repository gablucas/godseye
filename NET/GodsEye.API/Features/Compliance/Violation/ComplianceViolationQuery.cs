using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;

namespace GodsEye.API.Features.Compliance.Violation
{
    public interface IComplianceViolationQuery
    {
        Task<IEnumerable<ComplianceViolationResponse>> GetAllComplianceViolationQuery(int pageNumber, int pageSize, CancellationToken cancellationToken);
    }

    public class ComplianceViolationQuery(IDapperContext context) : IComplianceViolationQuery
    {
        public async Task<IEnumerable<ComplianceViolationResponse>> GetAllComplianceViolationQuery(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_VIOLATION_GET_ALL(@P_PAGE_NUMBER, @P_PAGE_SIZE)";

            var parameters = new
            {
                P_PAGE_NUMBER = pageNumber,
                P_PAGE_SIZE = pageSize
            };

            return await context.QuerySqlAsync<ComplianceViolationResponse>(sql, parameters, cancellationToken);
        }
    }
}
