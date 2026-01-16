using GodsEye.Application.UseCases.Person.Queries.GetAllPersons;
using GodsEye.Application.UseCases.Sector.Commands.CreateSector;
using GodsEye.Application.UseCases.Sector.Queries.GetAllSectors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SectorController : ControllerBase
    {
        private IMediator _mediator;

        public SectorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateSector([FromBody] CreateSectorRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllSectors(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllSectorsRequest(), cancellationToken);
            return Ok(result);
        }
    }
}
