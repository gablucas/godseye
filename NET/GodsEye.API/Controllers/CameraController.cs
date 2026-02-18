using GodsEye.Application.UseCases.Camera.Commands.CreateCamera;
using GodsEye.Application.UseCases.Camera.Commands.CreateCameraConfigDwellTimeMonitoring;
using GodsEye.Application.UseCases.Camera.Commands.CreateCameraRoi;
using GodsEye.Application.UseCases.Camera.Commands.DeleteCameraRoi;
using GodsEye.Application.UseCases.Camera.Commands.GetCameraConfigDwellTimeMonitoring;
using GodsEye.Application.UseCases.Camera.Commands.UpdateCamera;
using GodsEye.Application.UseCases.Camera.Commands.UpdateCameraConfigDwellTimeMonitoring;
using GodsEye.Application.UseCases.Camera.Commands.UpdateCameraIncidentRecording;
using GodsEye.Application.UseCases.Camera.Commands.UpdateCameraRoi;
using GodsEye.Application.UseCases.Camera.Queries.GetAllCameras;
using GodsEye.Application.UseCases.Camera.Queries.GetAllCamerasConnection;
using GodsEye.Application.UseCases.Camera.Queries.GetCameraById;
using GodsEye.Application.UseCases.Camera.Queries.GetCameraFeatureById;
using GodsEye.Application.UseCases.Camera.Queries.GetCamerasByFeatureID;
using GodsEye.Application.UseCases.Camera.Queries.GetCamerasRoiByCameraId;
using GodsEye.Application.UseCases.Camera.Queries.TestCameraConnection;
using GodsEye.Application.UseCases.IncidentRecording.Commands.UpdateIncidentRecordingLog;
using GodsEye.Application.UseCases.Person.Queries.GetCameraLog;
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCameraByIdRequest(id), cancellationToken);
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
        [HttpGet("feature/{id}")]
        public async Task<IActionResult> GetCamerasByFeatureId(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCamerasByFeatureIdRequest(id), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("logs/{cameraId}")]
        public async Task<IActionResult> GetCameraLogs([FromRoute] GetCameraLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("active-features/{cameraId}")]
        public async Task<IActionResult> GetCameraLogs([FromRoute] GetCameraFeatureByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<IActionResult> UpdateCamera([FromBody] UpdateCameraRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("test-connection")]
        public async Task<IActionResult> CheckCameraConnection(CheckCameraConnectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("roi/{cameraId}")]
        public async Task<IActionResult> GetCameraRoiByCameraId(int cameraId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCamerasRoiByCameraIdRequest(cameraId), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("roi")]
        public async Task<IActionResult> CreateCameraRoi([FromBody] CreateCameraRoiRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPut("roi")]
        public async Task<IActionResult> UpdateRoiCamera(UpdateCameraRoiRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpDelete("roi/{id}")]
        public async Task<IActionResult> DeleteRoiCamera(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteCameraRoiRequest(id), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPut("incident-recording")]
        public async Task<IActionResult> UpdateIncidentRecordingCamera([FromBody] UpdateCameraIncidentRecordingRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("config/dwell-time-monitoring/{id}")]
        public async Task<IActionResult> GetConfigDwellTimeMonitoring(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCameraConfigDwellTimeMonitoringRequest(id), cancellationToken);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("config/dwell-time-monitoring")]
        public async Task<IActionResult> CreateConfigDwellTimeMonitoring([FromBody] CreateCameraConfigDwellTimeMonitoringRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            if (result.Success)
                return Ok(result);
            else 
                return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPut("config/dwell-time-monitoring")]
        public async Task<IActionResult> UpdateRoiCamera(UpdateCameraConfigDwellTimeMonitoringRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
