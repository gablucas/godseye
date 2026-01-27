using AutoMapper;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;

namespace GodsEye.Application.UseCases.DwellTimeMonitoring.Commands.CreateDwellTimeMonitoring
{
    public class CreateDwellTimeMonitoringHandler : IRequestHandler<CreateDwellTimeMonitoringRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IMapper _mapper;
        private readonly IDwellTimeMonitoringRepository _dwellTimeMonitoringRepository;

        public CreateDwellTimeMonitoringHandler(IMapper mapper, IDwellTimeMonitoringRepository dwellTimeMonitoringRepository)
        {
            _mapper = mapper;
            _dwellTimeMonitoringRepository = dwellTimeMonitoringRepository;
        }
        public async Task<ApiResponse<ProcedureResult>> Handle(CreateDwellTimeMonitoringRequest request, CancellationToken cancellationToken)
        {
            var dwellTimeMonitoring = _mapper.Map<DwellTimeMonitoringEntity>(request);

            var result = await _dwellTimeMonitoringRepository.Create(dwellTimeMonitoring, cancellationToken);

            if (result is null || result.Erro == 1)
                throw new InvalidOperationException("Falha no registro do controle de permanência");

            return ApiResponse<ProcedureResult>.Ok(result);
        }   
    }
}
