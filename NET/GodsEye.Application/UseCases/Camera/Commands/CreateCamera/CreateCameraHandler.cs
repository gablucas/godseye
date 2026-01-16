using AutoMapper;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.CreateCamera
{
    public class CreateCameraHandler : IRequestHandler<CreateCameraRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IMapper _mapper;
        private readonly ICameraRepository _cameraRepository;

        public CreateCameraHandler(IMapper mapper, ICameraRepository cameraRepository)
        {
            _mapper = mapper;
            _cameraRepository = cameraRepository;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateCameraRequest request, CancellationToken cancellationToken)
        {
            var newCamera = _mapper.Map<CameraEntity>(request);

            var result = await _cameraRepository.Create(newCamera, cancellationToken);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
