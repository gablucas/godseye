using GodsEye.Application.DTOs.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Serialization;

namespace GodsEye.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CameraEventController : Controller
    {
        private readonly IMediator _mediator;

        public CameraEventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("hikvision")]
        public async Task<IActionResult> ReceiveHikvisionEvent(CancellationToken cancellationToken)
        {
            if (!Request.HasFormContentType)
                return BadRequest("Conteúdo não é multipart/form-data");

            var file = Request.Form.Files.FirstOrDefault();

            if (file == null)
                return BadRequest("Arquivo XML não encontrado");

            string xml;

            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                xml = await reader.ReadToEndAsync();
            }

            var serializer = new XmlSerializer(
                typeof(HikvisionEventNotificationAlert)
            );

            using var stringReader = new StringReader(xml);

            var evento = (HikvisionEventNotificationAlert)
                serializer.Deserialize(stringReader);

            //var result = await _mediator.Send(new CreateIncidentRecordingLogRequest(evento.MacAddress));

            //// DEBUG
            //Console.WriteLine($"ID Camera: {evento.ChannelID}");
            //Console.WriteLine($"Nome Camera: {evento.ChannelName}");
            //Console.WriteLine($"Tipo Evento: {evento.EventType}");
            //Console.WriteLine($"Input Port ID: {evento.InputIOPortID}");
            //Console.WriteLine($"Estado do Evento: {evento.EventState}");
            //Console.WriteLine($"Time: {evento.DateTime}");
            //Console.WriteLine($"Evento recebido: {evento.EventType}");
            //Console.WriteLine($"Input físico: {evento.InputIOPortID}");

            return Ok();
        }
    }
}
