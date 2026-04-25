using GodsEye.Application.DTOs.Model;
using GodsEye.Domain.DTOs.Result;

namespace GodsEye.Application.Interfaces.Write
{
    public interface IComplianceWrite
    {
        Task<ProcedureResult?> CreateSectorTransitionRule(string policyName, List<SectorTransitionRuleDTO> rule, CancellationToken cancellationToken);
        Task<ProcedureResult?> CreateLog(int personId, int sectorId, DateTime identifiedAt, CancellationToken cancellationToken);
    }
}
