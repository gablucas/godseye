using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using GodsEye.Application.UseCases.GodsEye.Commands.StartStream;
using GodsEye.Application.UseCases.GodsEye.Queries.GetMonitoringData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GodsEyeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;

        public GodsEyeController(IMediator mediator, IEmailService emailService)
        {
            _mediator = mediator;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("start-stream")]
        public async Task<IActionResult> StartStream([FromBody] CameraModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new StartStreamRequest(request.Name, request.Connection), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetMonitoringData(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMonitoringDataRequest(), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("email")]
        public async Task<IActionResult> TestEmail(CancellationToken cancellationToken)
        {
            var html = await _emailService.LoadTemplateAsync(
                "IncidentRecordingAlert.html",
                new Dictionary<string, string>
                {
                    ["camera"] = "teste",
                    ["date"] = "2025",
                    ["videoUrl"] = "URL"
                }
            );

            await _emailService.SendAsync(["gabriel.pegoretti96@gmail.com"], "Teste", html);
            return Ok("FOI");
        }
    }
}
