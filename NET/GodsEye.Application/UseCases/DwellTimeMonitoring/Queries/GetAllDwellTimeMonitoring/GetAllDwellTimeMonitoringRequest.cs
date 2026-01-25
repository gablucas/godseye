using GodsEye.Application.DTOs.Model;
using MediatR;

namespace GodsEye.Application.UseCases.DwellTimeMonitoring.Queries.GetAllDwellTimeMonitoring
{
    public sealed record GetAllDwellTimeMonitoringRequest() : IRequest<List<DwellTimeMonitoringModel>>;
}
