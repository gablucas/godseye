using GodsEye.Application.UseCases.Device.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : Controller
    {
        private readonly IMediator _mediator;

        public DeviceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> BiometricScanner(CancellationToken cancellationToken)
        {
            var date = DateTime.Now;
            var result = await _mediator.Send(new RegisterPersonByBiometricScannerRequest(41, 7, date), cancellationToken);
            return Ok(result);
        }
    }
}
