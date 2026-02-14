using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Queries.GetAllIncidentRecordingLogs
{
    public class GetAllIncidentRecordingLogsHandler : IRequestHandler<GetAllIncidentRecordingLogsRequest, ApiResponse<IEnumerable<IncidentRecordingModel>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllIncidentRecordingLogsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<IncidentRecordingModel>>> Handle(GetAllIncidentRecordingLogsRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_INCIDENT_RECORDING_GET_ALL_LOGS()";

            var result = await _context.QuerySqlAsync<IncidentRecordingModel>(sql, cancellationToken);

            return ApiResponse<IEnumerable<IncidentRecordingModel>>.Ok(result);
        }
    }
}
