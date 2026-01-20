using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using GodsEye.Application.UseCases.IncidentRecording.Queries.GetIncidentRecordingToProcessingLogs;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Queries.GetIncidentRecordingToProcessing
{
    public class GetIncidentRecordingToProcessingHandler : IRequestHandler<GetIncidentRecordingToProcessingRequest, ApiResponse<IncidentRecordingProcessModel>>
    {
        private readonly IIncidentRecordingQueryRepository _incidentRecordingQueryRepository;

        public GetIncidentRecordingToProcessingHandler(IIncidentRecordingQueryRepository incidentRecordingQueryRepository)
        {
            _incidentRecordingQueryRepository = incidentRecordingQueryRepository;
        }

        public async Task<ApiResponse<IncidentRecordingProcessModel>> Handle(GetIncidentRecordingToProcessingRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentRecordingQueryRepository.GetToProcess(cancellationToken);

            if (result is null || result.Id is null)
            {
                return ApiResponse<IncidentRecordingProcessModel?>.Ok(null);
            }

            return ApiResponse<IncidentRecordingProcessModel>.Ok(result);
        }
    }
}
