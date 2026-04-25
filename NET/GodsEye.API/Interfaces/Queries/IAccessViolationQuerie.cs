using GodsEye.API.DTO;

namespace GodsEye.API.Services.Queries
{
    public interface IAccessViolationQuerie
    {
        Task<AccessViolationDetailDTO?> GetAccessViolationDetail(int personId, int sectorId, CancellationToken cancellationToken);
    }
}
