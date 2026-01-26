using GodsEye.Application.UseCases.DwellTimeMonitoring.Commands.CreateDwellTimeMonitoring;
using GodsEye.Application.UseCases.DwellTimeMonitoring.Queries.GetDwellTimeMonitoringDetailsByCameraId;
using GodsEye.Application.UseCases.EnvironmentMonitoring.Commands.CreateEnvironmentMonitoringLog;
using GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetAllEnvironmentMonitoringLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DwellTimeMonitoringController : Controller
    {
        private readonly IMediator _mediator;

        public DwellTimeMonitoringController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(CreateDwellTimeMonitoringRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("details/{cameraId}")]
        public async Task<IActionResult> GetDetailsByCameraId(int cameraId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDwellTimeMonitoringDetailsByCameraIdRequest(cameraId), cancellationToken);
            return Ok(result);
        }
    }
}
