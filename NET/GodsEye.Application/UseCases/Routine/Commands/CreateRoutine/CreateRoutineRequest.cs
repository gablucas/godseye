using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.Enums;
using MediatR;

namespace GodsEye.Application.UseCases.Routine.Commands.CreateRoutine
{
    public class CreateRoutineRequest : IRequest<ApiResponse<int>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoutineRuleTypeEnum RuleType { get; set; }
        public List<CreateRoutineRuleDTO> Rules { get; set; }
    }

    public class CreateRoutineRuleDTO
    {
        public int OrderIndex { get; set; }
        public int? MinTime { get; set; }
        public int? MaxTime { get; set; }
        public int? SectorId { get; set; }
    }
}
