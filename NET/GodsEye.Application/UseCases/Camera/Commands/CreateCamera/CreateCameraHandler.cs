using AutoMapper;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Camera.Commands.CreateCamera
{
    public class CreateCameraHandler : IRequestHandler<CreateCameraRequest, ApiResponse<ProcedureResult>>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;

        public CreateCameraHandler(IMapper mapper, IApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ApiResponse<ProcedureResult>> Handle(CreateCameraRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_CAMERA_CREATE(@P_NAME, @P_CONNECTION, @P_SECTOR_ID, @P_FEATURES_JSON)";

            var parameters = new
            {
                P_NAME = request.Name,
                P_CONNECTION = request.Connection,
                P_FEATURES_JSON = request.Features

            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            return ApiResponse<ProcedureResult>.Ok(result);
        }
    }
}
