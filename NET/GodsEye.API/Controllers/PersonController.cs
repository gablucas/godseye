using GodsEye.Application.UseCases.Person.Commands.CreatePerson;
using GodsEye.Application.UseCases.Person.Queries.GetAllPersonEmbedding;
using GodsEye.Application.UseCases.Person.Queries.GetAllPersons;
using GodsEye.Application.UseCases.Person.Queries.GetPersonById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePerson([FromForm] string Name, [FromForm] string Photo, [FromForm] int SectorId, [FromForm] int AccessLevelId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(Photo))
                return BadRequest("Foto inválida.");

            // Remove prefixo "data:image/png;base64,"
            var base64 = Photo.Contains(",")
                ? Photo.Split(",")[1]
                : Photo;

            var photoBytes = Convert.FromBase64String(base64);

            var result = await _mediator.Send(new CreatePersonRequest(Name, photoBytes, SectorId, AccessLevelId));
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllPerson(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllPersonRequest(), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{personId}")]
        public async Task<IActionResult> GetById(int personId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPersonByIdRequest(personId), cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("embedding")]
        public async Task<IActionResult> GetAllPersonEmbedding(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllPersonsEmbeddingRequest(), cancellationToken);
            return Ok(result);
        }
    }
}
