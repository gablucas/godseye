using GodsEye.Application.UseCases.Camera.Commands.CreateCamera;
using GodsEye.Application.UseCases.Camera.Queries.GetAllCameras;
using GodsEye.Application.UseCases.Camera.Queries.GetAllCamerasConnection;
using GodsEye.Application.UseCases.Person.Queries.GetCameraLog;
using GodsEye.Application.UseCases.Person.Queries.GetPersonLog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CameraController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CameraController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateCamera([FromBody] CreateCameraRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllCameras(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllCamerasRequest(), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("connection")]
        public async Task<IActionResult> GetAllCamerasConnection(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllCamerasConnectionRequest(), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("logs/{cameraId}")]
        public async Task<IActionResult> GetCameraLogs([FromRoute] GetCameraLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
