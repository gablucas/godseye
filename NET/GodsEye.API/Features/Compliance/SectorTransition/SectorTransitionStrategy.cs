using GodsEye.API.Features.Compliance.Shared;
using GodsEye.Shared.Enums;
using GodsEye.Shared.Response.Compliance;
using Hangfire;

namespace GodsEye.API.Features.Compliance.SectorTransition
{
    public class SectorTransitionStrategy(
        IComplianceLogService complianceLogService, 
        IBackgroundJobClient backgroundJobs,
        ISectorTransitionQuery sectorTransitionQuery

        ) : IComplianceStrategy
    {
        public CompliancePolicyEnum RuleType => CompliancePolicyEnum.SECTOR_TRANSITION;

        public async Task Apply(int complianceLogId, int personId, int sectorId, CompliancePolicyDTO policy, CancellationToken cancellationToken)
        {
            var sectorTransitionRules = await sectorTransitionQuery.GetRuleById(policy.Id, cancellationToken);
            var logs = await complianceLogService.GetByPersonId(personId, cancellationToken);

            var currentRule = sectorTransitionRules.Rules.FirstOrDefault(x => x.SectorId == sectorId);

            if (currentRule is null)
                return;

            var logIsDone = logs.FirstOrDefault(x => x.SectorId == sectorId && x.PersonId == personId && x.ExitedAt.HasValue && x.Id == complianceLogId);
            if (logIsDone is not null)
            {
                var nextRule = sectorTransitionRules.Rules.FirstOrDefault(x => x.OrderIndex == (currentRule.OrderIndex + 1));

                if (nextRule is not null)
                {
                    backgroundJobs.Schedule<SectorTransitionService>(
                    s => s.ValidateNextSector(complianceLogId, policy.Id, personId, logIsDone, nextRule.SectorId),
                    TimeSpan.FromMinutes(1));
                }

                return;
            }

            var hasOtherActiveLog = logs.FirstOrDefault(x => x.SectorId == sectorId && x.PersonId == personId && x.ExitedAt is null && x.Id != complianceLogId);
            if (hasOtherActiveLog is not null)
                return;

            if (currentRule.OrderIndex == 1)
            {
                await ScheduleCheck(complianceLogId, personId, policy, currentRule);
                return;
            }

            var previousRules = sectorTransitionRules.Rules
                .Where(x => x.OrderIndex < currentRule.OrderIndex)
                .OrderBy(x => x.OrderIndex)
                .ToList();

            var firstRule = sectorTransitionRules.Rules.First(x => x.OrderIndex == 1);

            var cycleStart = logs
                .Where(x => x.SectorId == firstRule.SectorId && x.ExitedAt is not null)
                .OrderByDescending(x => x.ExitedAt)
                .FirstOrDefault()?.EnteredAt ?? DateTime.MinValue;

            var currentCycleLogs = logs
                .Where(x => x.EnteredAt >= cycleStart)
                .ToList();

            var passedAllPrevious = previousRules.All(rule =>
                currentCycleLogs.Any(log =>
                    log.SectorId == rule.SectorId));

            if (!passedAllPrevious)
                return;

            await ScheduleCheck(complianceLogId, personId, policy, currentRule);
        }

        private async Task ScheduleCheck(int complianceLogId, int personId, CompliancePolicyDTO policy, SectorTransitionRuleResponse rule)
        {
            if (rule.MinTime.HasValue)
            {
                var minTime = rule.MinTime.Value;

                backgroundJobs.Schedule<SectorTransitionService>(
                s => s.ValidateMinTime(complianceLogId, policy.Id, personId, minTime),
                TimeSpan.FromMinutes(minTime));
            }

            if (rule.MaxTime.HasValue)
            {
                var maxTime = rule.MaxTime.Value;

                backgroundJobs.Schedule<SectorTransitionService>(
                s => s.ValidateMaxTime(complianceLogId, policy.Id, personId),
                TimeSpan.FromMinutes(maxTime));
            }
        }
    }
}
