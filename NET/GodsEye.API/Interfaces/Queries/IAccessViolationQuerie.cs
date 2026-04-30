using GodsEye.API.DTO;

namespace GodsEye.API.Services.Queries
{
    public interface IAccessViolationQuerie
    {
        Task<AccessViolationDetailResponse?> GetAccessViolationDetail(int personId, int sectorId, CancellationToken cancellationToken);
    }
}
