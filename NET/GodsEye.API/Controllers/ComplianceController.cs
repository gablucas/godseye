using GodsEye.Application.UseCases.Compliance.Commands;
using GodsEye.Application.UseCases.Compliance.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplianceController : Controller
    {
        private readonly IMediator _mediator;

        public ComplianceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetAllComplianceRequest(), cancellationToken);
        //    return Ok(result);
        //}

        //[AllowAnonymous]
        //[HttpGet("{Id}")]
        //public async Task<IActionResult> GetAll([FromRoute] int Id, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetComplianceByIdRequest(Id), cancellationToken);
        //    return Ok(result);
        //}

        //[AllowAnonymous]
        //[HttpPost("rule/sector-transitions")]
        //public async Task<IActionResult> CreateSectorTransitionRule([FromBody] CreateSectorTransitionRuleRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(request, cancellationToken);
        //    return Ok(result);
        //}
    }
}
