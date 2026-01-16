using GodsEye.Application.UseCases.EnvironmentMonitoring.Commands.CreateEnvironmentMonitoringLog;
using GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetAllEnvironmentMonitoringLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentMonitoringController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EnvironmentMonitoringController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllEnvironmentMonitoringLogsRequest(), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GenerateLog(CreateEnvironmentMonitoringLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
