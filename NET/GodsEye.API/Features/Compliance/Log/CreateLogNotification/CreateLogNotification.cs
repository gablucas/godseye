using GodsEye.API.Features.Compliance.Shared;
using GodsEye.API.Interfaces;
using GodsEye.Shared.Response.Person;
using MediatR;

namespace GodsEye.API.Features.Compliance.Log.CreateLogNotification
{
    internal sealed record CreateComplianceLogNotification(int personId, int sectorId, DateTime identifiedAt) : INotification;

    internal sealed class CreateComplianceLogHandler(IDapperContext context, IEnumerable<IComplianceStrategy> complianceStrategy, ILogger<CreateComplianceLogHandler> logger) : INotificationHandler<CreateComplianceLogNotification>
    {
        public async Task Handle(CreateComplianceLogNotification request, CancellationToken cancellationToken)
        {
            var result = await CreateComplianceLogWrite(request, cancellationToken);

            if (result is null || result.Id == 0)
            {
                string message = "Houve um erro ao inserir o log do compliance";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            var sectorPolicies = await GetSectorPolicies(request.sectorId, cancellationToken);

            if (result is null)
            {
                string message = "Houve um erro ao buscar as politicas do setor";
                logger.LogInformation(message);
                throw new InvalidOperationException(message);
            }

            foreach (var sectorPolicy in sectorPolicies) 
            {
                var service = complianceStrategy.FirstOrDefault(s => s.RuleType == sectorPolicy.Rule);

                if (service is null)
                {
                    logger.LogWarning("Nenhuma strategy encontrada para RuleType {RuleType}", sectorPolicy.Rule);
                    continue;
                }

                await service.Apply(result.Id, request.personId, request.sectorId, sectorPolicy, cancellationToken);
            }
        }


        private async Task<PersonResponse?> CreateComplianceLogWrite(CreateComplianceLogNotification request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_LOG_CREATE(@P_PERSON_ID, @P_SECTOR_ID, @P_IDENTIFIED_AT)";

            var parameters = new
            {
                P_PERSON_ID = request.personId,
                P_SECTOR_ID = request.sectorId,
                P_IDENTIFIED_AT = request.identifiedAt
            };

            var result = await context.QuerySingleSqlAsync<PersonResponse>(sql, parameters, cancellationToken);

            return result;
        }

        private async Task<IEnumerable<CompliancePolicyDTO>> GetSectorPolicies(int sectorId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_COMPLIANCE_GET_BY_DEVICE(@P_SECTOR_ID)";

            var parameters = new
            {
                P_SECTOR_ID = sectorId,
            };

            var result = await context.QuerySqlAsync<CompliancePolicyDTO>(sql, parameters, cancellationToken);

            return result;
        }
    }
}
