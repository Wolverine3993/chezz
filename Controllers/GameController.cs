using Microsoft.AspNetCore.Mvc;

namespace Chezz.Controllers;

public class GameController : ControllerBase
{
    [Route("/ws")]
    public async Task Get()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = 400;
            return;
        }
        
        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        
    }
}