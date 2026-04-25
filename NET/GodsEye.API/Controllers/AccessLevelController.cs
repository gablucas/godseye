using GodsEye.Application.UseCases.AccessLevel.Commands.CreateOrUpdateAccessLevel;
using GodsEye.Application.UseCases.AccessLevel.Queries.GetAccessLevelById;
using GodsEye.Application.UseCases.AccessLevel.Queries.GetAllAcessLevel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessLevelController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccessLevelController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateOrUpdateAccessLevel([FromBody] CreateOrUpdateAccessLevelRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(request, cancellationToken);
        //    return Ok(result);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GettAllAccessLevel(CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetAllAccessLevelRequest(), cancellationToken);
        //    return Ok(result);
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GettAllAccessLevel(int Id, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetAccessLevelByIdRequest(Id), cancellationToken);
        //    return Ok(result);
        //}
    }
}
