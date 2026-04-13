using GodsEye.Application.UseCases.Routine.Commands.CreateRoutine;
using GodsEye.Application.UseCases.Routine.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutineController : ControllerBase
    {

        private readonly IMediator _mediator;

        public RoutineController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateRoutine([FromBody] CreateRoutineRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllRoutines(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllRoutinesRequest(), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{routineId}")]
        public async Task<IActionResult> GetById(int routineId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetRoutineByIdRequest(routineId), cancellationToken);
            return Ok(result);
        }
    }
}
