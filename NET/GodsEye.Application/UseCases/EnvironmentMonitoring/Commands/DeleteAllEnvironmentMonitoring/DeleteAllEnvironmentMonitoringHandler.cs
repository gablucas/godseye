using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.EnvironmentMonitoring.Commands.DeleteAllEnvironmentMonitoring
{
    public class DeleteAllEnvironmentMonitoringHandler : IRequestHandler<DeleteAllEnvironmentMonitoringRequest, ApiResponse<bool>>
    {
        private readonly IApplicationDbContext _context;

        public DeleteAllEnvironmentMonitoringHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteAllEnvironmentMonitoringRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_ENVIRONMENT_MONITORING_DELETE_ALL()";

            var parameters = new { };

            await _context.ExecuteDeleteAsync(sql, parameters, cancellationToken);
            return ApiResponse<bool>.Ok(true);
        }
    }
}
