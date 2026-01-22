using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Queries.GetIncidentRecordingLogById
{
    public class GetIncidentRecordingLogByIdHandler : IRequestHandler<GetIncidentRecordingLogByIdRequest, ApiResponse<IncidentRecordingModel>>
    {
        private readonly IIncidentRecordingQueryRepository _incidentRecordingQueryRepository;

        public GetIncidentRecordingLogByIdHandler(IIncidentRecordingQueryRepository incidentRecordingQueryRepository)
        {
            _incidentRecordingQueryRepository = incidentRecordingQueryRepository;
        }

        public async Task<ApiResponse<IncidentRecordingModel>> Handle(GetIncidentRecordingLogByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentRecordingQueryRepository.GetByLogId(request.logId, cancellationToken);
            return ApiResponse<IncidentRecordingModel>.Ok(result);
        }
    }
}
