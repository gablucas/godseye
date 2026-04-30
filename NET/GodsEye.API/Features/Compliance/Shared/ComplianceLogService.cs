using GodsEye.API.Interfaces;

namespace GodsEye.API.Features.Compliance.Shared
{
    public interface IComplianceLogService
    {
        Task<IEnumerable<ComplianceLogDTO>> GetByPersonId(int personId, CancellationToken cancellationToken);
        Task<ComplianceLogDTO?> GetById(int complianceLogId, CancellationToken cancellationToken);
    }

    public class ComplianceLogService(IDapperContext context) : IComplianceLogService
    {
        public async Task<IEnumerable<ComplianceLogDTO>> GetByPersonId(int personId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_LOG_GET_BY_PERSON(@P_PERSON_ID)";

            return await context.QuerySqlAsync<ComplianceLogDTO>(sql, new { P_PERSON_ID = personId }, cancellationToken);
        }

        public async Task<ComplianceLogDTO?> GetById(int complianceLogId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_LOG_GET_BY_ID(@P_COMPLIANCE_LOG_ID)";

            return await context.QuerySingleSqlAsync<ComplianceLogDTO>(sql, new { P_COMPLIANCE_LOG_ID = complianceLogId }, cancellationToken);
        }
    }

}
