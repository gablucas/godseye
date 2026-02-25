using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Domain.DTOs.Result;
using MediatR;

namespace GodsEye.Application.UseCases.Sector.Commands.CreateSector
{
    public class CreateSectorHandler : IRequestHandler<CreateSectorRequest, ApiResponse<int>>
    {
        private readonly IApplicationDbContext _context;

        public CreateSectorHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<int>> Handle(CreateSectorRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_SECTOR_CREATE(@P_NAME, @P_NOTIFICATION_GROUP_JSON)";

            var parameters = new
            {
                P_NAME = request.Name,
                P_NOTIFICATION_GROUP_JSON = request.NotificationGroups
            };

            var result = await _context.QuerySingleSqlAsync<ProcedureResult>(sql, parameters, cancellationToken);

            return ApiResponse<int>.Ok(result.Id);
        }
    }
}
