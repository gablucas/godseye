using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.IncidentRecording.Queries.GetAllIncidentRecordingLogs
{
    public class GetAllIncidentRecordingLogsHandler : IRequestHandler<GetAllIncidentRecordingLogsRequest, ApiResponse<IEnumerable<IncidentRecordingModel>>>
    {
        private readonly IIncidentRecordingQueryRepository _incidentRecordingQueryRepository;

        public GetAllIncidentRecordingLogsHandler(IIncidentRecordingQueryRepository incidentRecordingQueryRepository)
        {
            _incidentRecordingQueryRepository = incidentRecordingQueryRepository;
        }

        public async Task<ApiResponse<IEnumerable<IncidentRecordingModel>>> Handle(GetAllIncidentRecordingLogsRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentRecordingQueryRepository.GetAll(cancellationToken);

            return ApiResponse<IEnumerable<IncidentRecordingModel>>.Ok(result);
        }
    }
}
