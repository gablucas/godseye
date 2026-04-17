using GodsEye.Application.DTOs.Model;

namespace GodsEye.Application.Interfaces.Queries
{
    public interface IAccessViolationQuerie
    {
        Task<AccessViolationDetailDTO?> GetAccessViolationDetail(int personId, int sectorId, CancellationToken cancellationToken);
    }
}
