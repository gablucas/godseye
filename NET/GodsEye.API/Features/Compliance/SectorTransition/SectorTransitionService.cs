using GodsEye.API.Features.Compliance.Shared;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Enums;
using GodsEye.Shared.Response;

namespace GodsEye.API.Features.Compliance.SectorTransition
{
    public interface ISectorTransitionService
    {
        Task Execute(int complianceLogId, int personId, int policyId);
    }

    public class SectorTransitionService(IComplianceLogService complianceLogService, IComplianceViolationService complianceViolationService, INotificationSignalR notificationSignalR, ILogger<SectorTransitionService> logger) : ISectorTransitionService
    {
        public async Task Execute(int complianceLogId, int policyId, int personId)
        {

            var log = await complianceLogService.GetById(complianceLogId, CancellationToken.None);

            if (log == null)
                return;

            if (log.ExitedAt is null)
            {
                Console.WriteLine("Usuario ainda não saiu do setor");
                

                var violation = new ComplianceViolationDTO()
                {
                    LogId = complianceLogId,
                    PolicyId = policyId,
                    PersonId = personId,
                    Type = ComplianceViolationEnum.EXCEEDED_ALLOWED_TIME
                };

                var result = await complianceViolationService.Create(violation, CancellationToken.None);

                if (result == null || result.Id == 0)
                    logger.LogError($"Houve um erro ao cadastrar uma violação de compliance {complianceLogId}-{policyId}-{personId}");

                var notification = new ViolationAlertFeatureResponse() { Id = result.Id, Type = FeatureEnum.COMPLIANCE };

                await notificationSignalR.SendAlertNotification(notification);
                await notificationSignalR.SendCreatedComplianceViolationLog(result.Id);
            }

            return;
        }
    }
}
