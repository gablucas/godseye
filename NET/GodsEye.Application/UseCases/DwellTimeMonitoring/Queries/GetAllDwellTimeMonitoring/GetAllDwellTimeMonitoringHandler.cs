using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces.QueryRepositories;
using MediatR;

namespace GodsEye.Application.UseCases.DwellTimeMonitoring.Queries.GetAllDwellTimeMonitoring
{
    public class GetAllDwellTimeMonitoringHandler : IRequestHandler<GetAllDwellTimeMonitoringRequest, List<DwellTimeMonitoringModel>>
    {
        private readonly IDwellTimeMonitoringQueryRepository _dwellTimeMonitoringQueryRepository;

        public GetAllDwellTimeMonitoringHandler(IDwellTimeMonitoringQueryRepository dwellTimeMonitoringQueryRepository)
        {
            _dwellTimeMonitoringQueryRepository = dwellTimeMonitoringQueryRepository;
        }

        public async Task<List<DwellTimeMonitoringModel>> Handle(GetAllDwellTimeMonitoringRequest request, CancellationToken cancellationToken)
        {
            var result = await _dwellTimeMonitoringQueryRepository.GetAll(cancellationToken);

            return result;
        }
    }
}
