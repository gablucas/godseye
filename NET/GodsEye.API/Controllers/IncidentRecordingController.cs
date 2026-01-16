using GodsEye.Application.UseCases.IncidentRecording.Commands.CreateIncidentRecordingLog;
using GodsEye.Application.UseCases.IncidentRecording.Queries.GetAllIncidentRecordingLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentRecordingController : Controller
    {
        private readonly IMediator _mediator;

        public IncidentRecordingController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllIncidentRecordingLogsRequest(), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GenerateLog(CreateIncidentRecordingLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}
