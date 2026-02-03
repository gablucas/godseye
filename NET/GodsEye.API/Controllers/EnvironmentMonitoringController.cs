using GodsEye.Application.UseCases.EnvironmentMonitoring.Commands.CreateEnvironmentMonitoringLog;
using GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetAllEnvironmentMonitoringLogs;
using GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetEnvironmentMonitoringLogsByPersonId;
using GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetEnvironmentMonitoringPersonsLastSector;
using GodsEye.Application.UseCases.EnvironmentMonitoring.Queries.GetEnvironmentMonitoringSectors;
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
        public async Task<IActionResult> GetAllLogs([FromQuery] int pageNumber, [FromQuery] int pageSize, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllEnvironmentMonitoringLogsRequest(pageNumber, pageSize), cancellationToken);
            return Ok(result);
        }

        [HttpGet("last-register-per-person")]
        public async Task<IActionResult> GetLastRegisterByPerson(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetEnvironmentMonitoringLastRegisterPerPerson(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("sectors")]
        public async Task<IActionResult> GetSectors(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetEnvironmentMonitoringSectorsRequest(), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GenerateLog(CreateEnvironmentMonitoringLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("person/{id}")]
        public async Task<IActionResult> GetByPersonId(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetEnvironmentMonitoringLogsByPersonIdRequest(id), cancellationToken);
            return Ok(result);
        }
    }
}
