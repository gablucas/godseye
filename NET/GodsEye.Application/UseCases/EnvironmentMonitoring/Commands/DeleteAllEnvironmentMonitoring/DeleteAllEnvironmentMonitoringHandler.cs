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

            var parameters = new Dictionary<string, object?>();

            await _context.ExecuteDeleteAsync("CALL SP_ENVIRONMENT_MONITORING_DELETE_ALL()", parameters, cancellationToken);
            return ApiResponse<bool>.Ok(true);
        }
    }
}
