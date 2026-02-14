using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Queries.GetIncidentRecordingLogById
{
    public class GetIncidentRecordingLogByIdHandler : IRequestHandler<GetIncidentRecordingLogByIdRequest, ApiResponse<IncidentRecordingModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetIncidentRecordingLogByIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IncidentRecordingModel>> Handle(GetIncidentRecordingLogByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await GetLogById(request.logId, cancellationToken);
            return ApiResponse<IncidentRecordingModel>.Ok(result);
        }

        private async Task<IncidentRecordingModel?> GetLogById(int logId, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_INCIDENT_RECORDING_GET_LOG_BY_ID(@P_ID)";

            var parameters = new
            {
                P_ID = logId
            };

            return await _context.QuerySingleSqlAsync<IncidentRecordingModel>(sql, parameters, cancellationToken);
        }
    }
}
