using GodsEye.Application.DTOs.Request;
using GodsEye.Application.UseCases.Person.Commands.CreatePerson;
using GodsEye.Application.UseCases.Person.Commands.CreateRecognize;
using GodsEye.Application.UseCases.Person.Commands.UpdatePerson;
using GodsEye.Application.UseCases.Person.Queries.GetAllPersonEmbedding;
using GodsEye.Application.UseCases.Person.Queries.GetAllPersons;
using GodsEye.Application.UseCases.Person.Queries.GetPersonById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        //[HttpPost]
        //public async Task<IActionResult> CreatePerson(CreatePersonRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(request, cancellationToken);
        //    return Ok(result);
        //}

        //[HttpPut]
        //public async Task<IActionResult> UpdatePerson(UpdatePersonRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(request, cancellationToken);
        //    return Ok(result);
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetAllPerson(CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetAllPersonRequest(), cancellationToken);
        //    return Ok(result);
        //}

        //[HttpGet("{personId}")]
        //public async Task<IActionResult> GetById(int personId, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetPersonByIdRequest(personId), cancellationToken);
        //    return Ok(result);
        //}

        //[HttpGet("embedding")]
        //public async Task<IActionResult> GetAllPersonEmbedding(CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(new GetAllPersonsEmbeddingRequest(), cancellationToken);
        //    return Ok(result);
        //}

        //[HttpPost("recognize")]
        //public async Task<IActionResult> CreatePersonRecognize([FromBody] PersonRecognizeRequest request, CancellationToken cancellationToken)
        //{
        //    if (string.IsNullOrWhiteSpace(request.Photo))
        //        return BadRequest("Foto inválida.");

        //    // Remove prefixo "data:image/png;base64,"
        //    var base64 = request.Photo.Contains(",")
        //        ? request.Photo.Split(",")[1]
        //        : request.Photo;

        //    var photoBytes = Convert.FromBase64String(base64);

        //    var result = await _mediator.Send(new CreateRecognizeRequest(request.PersonId, photoBytes), cancellationToken);
        //    return Ok(result);
        //}

    }
}
