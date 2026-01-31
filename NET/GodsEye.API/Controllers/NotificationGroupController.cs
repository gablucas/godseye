using GodsEye.Application.UseCases.NotificationGroup.Commands.CreateNotificationGroup;
using GodsEye.Application.UseCases.NotificationGroup.Commands.UpdateNotificationGroup;
using GodsEye.Application.UseCases.NotificationGroup.Queries.GetAllNotificationGroups;
using GodsEye.Application.UseCases.NotificationGroup.Queries.GetNotificationGroupById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationGroupController : Controller
    {
        private readonly IMediator _mediator;

        public NotificationGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllNotificationGroupsRequest(), cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetNotificationGroupByIdRequest(id), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNotificationGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateNotificationGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
