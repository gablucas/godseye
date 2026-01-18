using GodsEye.Application.UseCases.Feature.Queries.GetAllFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureController : Controller
    {
        private readonly IMediator _mediator;

        public FeatureController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllFeaturesRequest(), cancellationToken);
            return Ok(result);
        }
    }
}
