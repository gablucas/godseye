using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.Interfaces.Write;
using GodsEye.Domain.DTOs.Result;
using System.Text.Json;

namespace GodsEye.Infrastructure.Write
{
    public class ComplianeWrite : IComplianceWrite
    {
        private readonly IDapperContext _context;

        public ComplianeWrite(IDapperContext context)
        {
            _context = context;
        }

        public async Task<ProcedureResult?> CreateSectorTransitionRule(string policyName, List<SectorTransitionRuleDTO> rule, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_RULE_SECTOR_TRANSITION_CREATE(@P_POLICY_NAME, @P_RULE_JSON)";

            var parameters = new
            {
                P_POLICY_NAME = policyName,
                P_RULE_JSON = JsonSerializer.Serialize(rule)
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            return result;
        }

        public async Task<ProcedureResult?> CreateLog(int personId, int sectorId, DateTime identifiedAt, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_LOG_CREATE(@P_PERSON_ID, @P_SECTOR_ID, @P_EVENT_TYPE, @P_IDENTIFIED_AT)";

            var parameters = new
            {
                P_PERSON_ID = personId,
                P_SECTOR_ID = sectorId,
                P_EVENT_TYPE = "ENTRY",
                P_IDENTIFIED_AT = identifiedAt
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            return result;
        }
    }
}
