using AutoMapper;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;

namespace GodsEye.Application.UseCases.Sector.Commands.CreateSector
{
    public class CreateSectorHandler : IRequestHandler<CreateSectorRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IMapper _mapper;
        private readonly ISectorRepository _sectorRepository;

        public CreateSectorHandler(IMapper mapper, ISectorRepository sectorRepository)
        {
            _mapper = mapper;
            _sectorRepository = sectorRepository;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateSectorRequest request, CancellationToken cancellationToken)
        {
            var newSector = _mapper.Map<SectorEntity>(request);
            var result = await _sectorRepository.Create(newSector, cancellationToken);
            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
