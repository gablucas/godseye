using GodsEye.API.Features.Compliance.Shared;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Enums;
using Hangfire;

namespace GodsEye.API.Features.Compliance.SectorTransition
{
    public class SectorTransitionStrategy(IDapperContext context, IComplianceLogService complianceLogService, IBackgroundJobClient backgroundJobs) : IComplianceStrategy
    {
        public ComplianceRuleEnum RuleType => ComplianceRuleEnum.SECTOR_TRANSITION;

        public async Task Apply(int complianceLogId, int personId, int sectorId, CompliancePolicyDTO policy, CancellationToken cancellationToken)
        {
            var rules = await GetRules(policy.Id, cancellationToken);
            var logs = await complianceLogService.GetByPersonId(personId, cancellationToken);

            var currentRule = rules.FirstOrDefault(x => x.SectorId == sectorId);

            if (currentRule is null)
                return;

            var activeLog = logs.FirstOrDefault(x => x.SectorId == sectorId && x.ExitedAt is null && x.Id != complianceLogId);
            if (activeLog is not null)
                return;

            if (currentRule.OrderIndex == 1)
            {
                await ScheduleCheck(complianceLogId, personId, policy, currentRule);
                return;
            }

            var previousRules = rules
                .Where(x => x.OrderIndex < currentRule.OrderIndex)
                .OrderBy(x => x.OrderIndex)
                .ToList();

            var firstSectorRule = rules.First(x => x.OrderIndex == 1);

            var cycleStart = logs
                .Where(x => x.SectorId == firstSectorRule.SectorId && x.ExitedAt is not null)
                .OrderByDescending(x => x.ExitedAt)
                .FirstOrDefault()?.ExitedAt ?? DateTime.MinValue;

            var currentCycleLogs = logs
                .Where(x => x.EnteredAt > cycleStart)
                .ToList();

            var passedAllPrevious = previousRules.All(rule =>
                currentCycleLogs.Any(log =>
                    log.SectorId == rule.SectorId));

            if (!passedAllPrevious)
                return;

            await ScheduleCheck(complianceLogId, personId, policy, currentRule);
        }

        private async Task ScheduleCheck(int complianceLogId, int personId, CompliancePolicyDTO policy, SectorTransitionDTO rule)
        {
            backgroundJobs.Schedule<SectorTransitionService>(
                s => s.Execute(complianceLogId, policy.Id, personId),
                TimeSpan.FromMinutes(rule.MaxTime));
        }

        public async Task<IEnumerable<SectorTransitionDTO>> GetRules(int policyId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_RULE_SECTOR_TRANSITION_GET_BY_POLICY_ID(@P_POLICY_ID)";

            var rules = await context.QuerySqlAsync<SectorTransitionDTO>(
            sql, new { P_POLICY_ID = policyId }, cancellationToken);

            return rules;
        }
    }

}
