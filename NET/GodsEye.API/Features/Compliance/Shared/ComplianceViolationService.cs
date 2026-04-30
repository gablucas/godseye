using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Person;


namespace GodsEye.API.Features.Compliance.Shared
{
    public interface IComplianceViolationService
    {
        Task<PersonResponse?> Create(ComplianceViolationDTO violation, CancellationToken cancellationToken);
    }

    public class ComplianceViolationService(IDapperContext context) : IComplianceViolationService
    {
        public async Task<PersonResponse?> Create(ComplianceViolationDTO violation, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_VIOLATION_CREATE(@P_LOG_ID, @P_POLICY_ID, @P_PERSON_ID, @P_VIOLATION_TYPE)";

            var parameters = new
            {
                P_LOG_ID = violation.LogId,
                P_POLICY_ID = violation.PolicyId,
                P_PERSON_ID = violation.PersonId,
                P_VIOLATION_TYPE = violation.Type
            };

            return await context.QuerySingleSqlAsync<PersonResponse>(sql, parameters, cancellationToken);
        }
    }

}
