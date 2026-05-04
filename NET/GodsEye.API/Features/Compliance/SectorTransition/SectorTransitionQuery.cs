using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Compliance;

namespace GodsEye.API.Features.Compliance.SectorTransition
{

    public interface ISectorTransitionQuery
    {
        Task<SectorTransitionResponse> GetRuleById(int policyId, CancellationToken cancellationToken);
        Task<IEnumerable<SectorTransitionResponse>> GetAll(CancellationToken cancellationToken);
    }

    public class SectorTransitionQuery : ISectorTransitionQuery
    {
        private readonly IDapperContext _context;

        public SectorTransitionQuery(IDapperContext context)
        {
            _context = context;
        }

        public async Task<SectorTransitionResponse> GetRuleById(int policyId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_RULE_SECTOR_TRANSITION_GET_BY_POLICY_ID(@P_POLICY_ID)";

            var rules = await _context.QuerySingleSqlAsync<SectorTransitionResponse>(
            sql, new { P_POLICY_ID = policyId }, cancellationToken);

            return rules;
        }

        public async Task<IEnumerable<SectorTransitionResponse>> GetAll(CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_RULE_SECTOR_TRANSITION_GET_ALL()";

            var rules = await _context.QuerySqlAsync<SectorTransitionResponse>(
            sql, new {  }, cancellationToken);

            return rules;
        }
    }
}
