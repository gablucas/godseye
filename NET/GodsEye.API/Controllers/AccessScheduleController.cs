using GodsEye.Application.UseCases.AccessSchedule.Commands.CreateAccessSchedule;
using GodsEye.Application.UseCases.AccessSchedule.Queries.GetAccessSchedulesById;
using GodsEye.Application.UseCases.AccessSchedule.Queries.GetAllAccessSchedules;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccessScheduleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[AllowAnonymous]
        //[HttpPost]
        //public async Task<IActionResult> CreateAccessSchedule([FromBody] CreateAccessScheduleRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(request, cancellationToken);
        //    return Ok(result);
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> GetAllAccessSchedules(CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetAllAccessSchedulesRequest(), cancellationToken);
        //    return Ok(result);
        //}

        //[AllowAnonymous]
        //[HttpGet("{Id}")]
        //public async Task<IActionResult> GetCameraLogs(int Id, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetAccessSchedulesByIdRequest(Id), cancellationToken);
        //    return Ok(result);
        //}
    }
}
