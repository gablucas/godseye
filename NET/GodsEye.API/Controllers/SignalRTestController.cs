using GodsEye.Application.DTOs.Model;
using GodsEye.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("test/log")]
public class LogTestController : ControllerBase
{
    private readonly INotificationSignalR _notification;

    public LogTestController(INotificationSignalR notification)
    {
        _notification = notification;
    }

    [HttpGet("send")]
    public async Task<IActionResult> Send()
    {
        return Ok("✔ Log enviado via SignalR");
    }
}
