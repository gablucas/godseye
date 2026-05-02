using GodsEye.API.Features.Compliance.Shared;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Enums;
using GodsEye.Shared.Response;

namespace GodsEye.API.Features.Compliance.SectorTransition
{
    public interface ISectorTransitionService
    {
        Task ValidateMaxTime(int complianceLogId, int personId, int policyId);
    }

    public class SectorTransitionService(IComplianceLogService complianceLogService, IComplianceViolationService complianceViolationService, INotificationSignalR notificationSignalR, ILogger<SectorTransitionService> logger) : ISectorTransitionService
    {
        public async Task ValidateMinTime(int complianceLogId, int policyId, int personId, int minTime)
        {

            var log = await complianceLogService.GetById(complianceLogId, CancellationToken.None);

            if (log == null)
                return;

            if (log.ExitedAt is DateTime exitedAt)
            {
                var duration = exitedAt - log.EnteredAt;

                if (duration.TotalMinutes < minTime)
                {
                    await NotificateViolation(complianceLogId, policyId, personId, ComplianceViolationEnum.BELOW_MINIMUM_TIME);
                }
            }

            return;
        }

        public async Task ValidateMaxTime(int complianceLogId, int policyId, int personId)
        {

            var log = await complianceLogService.GetById(complianceLogId, CancellationToken.None);

            if (log == null)
                return;

            if (log.ExitedAt is null)
            {
                await NotificateViolation(complianceLogId, policyId, personId, ComplianceViolationEnum.EXCEEDED_ALLOWED_TIME);
            }

            return;
        }

        public async Task ValidateNextSector(int complianceLogId, int policyId, int personId, ComplianceLogDTO currentLog, int nextSectorId)
        {

            var log = await complianceLogService.GetByPersonId(personId, CancellationToken.None);

            if (log == null)
                return;

            var isPersonInNextSector = log.FirstOrDefault(x => x.SectorId == nextSectorId && x.PersonId == personId && x.EnteredAt > currentLog.ExitedAt);

            if (isPersonInNextSector is null)
            {
                await NotificateViolation(complianceLogId, policyId, personId, ComplianceViolationEnum.DID_NOT_ENTER_NEXT_SECTOR_IN_TIME);
            }

            return;
        }

        private async Task NotificateViolation(int complianceLogId, int policyId, int personId, ComplianceViolationEnum tipoViolacao)
        {
            var violation = new ComplianceViolationDTO()
            {
                LogId = complianceLogId,
                PolicyId = policyId,
                PersonId = personId,
                Type = tipoViolacao
            };

            var result = await complianceViolationService.Create(violation, CancellationToken.None);

            if (result == null || result.Id == 0)
                logger.LogError($"Houve um erro ao cadastrar uma violação de compliance {complianceLogId}-{policyId}-{personId}");

            var notification = new ViolationAlertFeatureResponse() { Id = result.Id, Type = FeatureEnum.COMPLIANCE };

            await notificationSignalR.SendAlertNotification(notification);
            await notificationSignalR.SendCreatedComplianceViolationLog(result.Id);
        }
    }
}
