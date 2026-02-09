using GodsEye.Application.UseCases.MediaMtx.Commands.StartStream;
using GodsEye.Application.UseCases.MediaMtx.Queries.IsOnline;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaMtxController : Controller
    {
        public IMediator _mediator { get; set; }

        public MediaMtxController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("status")]
        public async Task<IActionResult> CheckStatus(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new IsOnlineRequest(), cancellationToken);
            return Ok(result);
        }


        [HttpPost("start-stream")]
        public async Task<IActionResult> StartStream(StartStreamRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
