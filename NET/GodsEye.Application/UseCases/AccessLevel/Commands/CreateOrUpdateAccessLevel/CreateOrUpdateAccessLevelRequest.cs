using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.Enums;
using MediatR;

namespace GodsEye.Application.UseCases.AccessLevel.Commands.CreateOrUpdateAccessLevel
{
    public class CreateOrUpdateAccessLevelRequest : IRequest<ApiResponse<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SectorAccessLevelInput> Sectors { get; set; } = new();
        public int? AccessScheduleId { get; set; }
        public IEnumerable<int> Routines { get; set; } = Enumerable.Empty<int>();
    }

    public class SectorAccessLevelInput
    {
        public SectorAccessLevelInput(int sectorId, AccessLevelSectorRuleEnum ruleType)
        {
            SectorId = sectorId;
            RuleType = ruleType;
        }

        public int SectorId { get; set; }
        public AccessLevelSectorRuleEnum RuleType { get; set; }
    }
}