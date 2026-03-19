using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces;
using GodsEye.Application.UseCases.IncidentRecording.Queries.GetIncidentRecordingToProcessingLogs;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Queries.GetIncidentRecordingToProcessing
{
    public class GetIncidentRecordingToProcessingHandler : IRequestHandler<GetIncidentRecordingToProcessingRequest, ApiResponse<IncidentRecordingProcessModel>>
    {
        private readonly IApplicationDbContext _context;

        public GetIncidentRecordingToProcessingHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IncidentRecordingProcessModel>> Handle(GetIncidentRecordingToProcessingRequest request, CancellationToken cancellationToken)
        {
            var sql = "CALL SP_INCIDENT_RECORDING_GET_TO_PROCESS()";
            var parameters = new { };

            var result = await _context.QuerySingleSqlAsync<IncidentRecordingProcessModel>(sql, parameters, cancellationToken);



            return ApiResponse<IncidentRecordingProcessModel>.Ok(result);
        }
    }
}
