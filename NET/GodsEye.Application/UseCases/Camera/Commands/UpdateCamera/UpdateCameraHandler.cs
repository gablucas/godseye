using AutoMapper;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Entities;
using GodsEye.Domain.Interfaces.Repositories;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.UpdateCamera
{
    public class UpdateCameraHandler : IRequestHandler<UpdateCameraRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IMapper _mapper;
        private readonly ICameraRepository _cameraRepository;

        public UpdateCameraHandler(IMapper mapper, ICameraRepository cameraRepository)
        {
            _mapper = mapper;
            _cameraRepository = cameraRepository;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(UpdateCameraRequest request, CancellationToken cancellationToken)
        {
            var updateCamera = _mapper.Map<CameraEntity>(request);

            var result = await _cameraRepository.Update(updateCamera, cancellationToken);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
