using Chezz.Database.EntityManagers;
using Chezz.Database.Models;
using Chezz.Notifications;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chezz.Controllers
{
    [Route("/api/notificationList")]
    public class NotificationController(
        UserManager<ChezzUser> _userManager, 
        NotificationManager _notificationManager,
        NotificationSocketRegistry _sockets) : ControllerBase
    {

        [HttpGet]
        public async Task<Results<Ok<Notification>, NotFound, UnauthorizedHttpResult>> GetNotifications()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }
            
            var earliestNotification = await _notificationManager.GetNotificationAsync(user);
            if (earliestNotification is null) return TypedResults.NotFound();

            return TypedResults.Ok(earliestNotification);
        }

        [Route("ws")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task ConnectNotifications()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            var connection = new NotificationSocket(webSocket);
            _sockets.Add(user.Id, connection);
            try
            {
                await connection.ListenUntilClosedAsync();
            }
            finally
            {
                _sockets.Remove(user.Id, connection);
            }
        }
    }
}
