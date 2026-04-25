using GodsEye.Application.Interfaces.Write;
using MediatR;

namespace GodsEye.Application.UseCases.Device.Command
{
    public sealed record RegisterPersonByBiometricScannerRequest(int personId, int sectorId, DateTime identifiedAt) : IRequest<int>;

    public class RegisterPersonByBiometricScannerHandler : IRequestHandler<RegisterPersonByBiometricScannerRequest, int>
    {
        private readonly IComplianceWrite _complianceWrite;

        public RegisterPersonByBiometricScannerHandler(IComplianceWrite complianceWrite)
        {
            _complianceWrite = complianceWrite;
        }

        public async Task<int> Handle(RegisterPersonByBiometricScannerRequest request, CancellationToken cancellationToken)
        {
            var result = await _complianceWrite.CreateLog(request.personId, request.sectorId, request.identifiedAt, cancellationToken);

            if (result == null)
                throw new InvalidOperationException("Houve um erro ao cadastrar a regra de compliance");

            return result.Id;

        }
    }
}
