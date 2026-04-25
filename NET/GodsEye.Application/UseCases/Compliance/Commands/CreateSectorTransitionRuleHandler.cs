using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.Write;
using MediatR;

namespace GodsEye.Application.UseCases.Compliance.Commands
{
    public class CreateSectorTransitionRuleRequest : IRequest<int>
    {
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public List<SectorTransitionRuleDTO> Rules { get; set; }
    }

    public class CreateSectorTransitionRuleHandler : IRequestHandler<CreateSectorTransitionRuleRequest, int>
    {
        private readonly IComplianceWrite _complianceWrite;

        public CreateSectorTransitionRuleHandler(IComplianceWrite complianceWrite)
        {
            _complianceWrite = complianceWrite;
        }

        public async Task<int> Handle(CreateSectorTransitionRuleRequest request, CancellationToken cancellationToken)
        {
            var result = await _complianceWrite.CreateSectorTransitionRule(request.PolicyName, request.Rules, cancellationToken);

            if (result == null)
                throw new InvalidOperationException("Houve um erro ao cadastrar a regra de compliance");

            return result.Id;
        }
    }
}
