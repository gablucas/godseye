using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.Queries
{
    public interface IComplianceQuerie
    {
        Task<IEnumerable<CompliancePolicyDTO>> GetAll(CancellationToken cancellationToken);
        Task<CompliancePolicyDTO?> GeById(int complianceId, CancellationToken cancellationToken);
    }
}
