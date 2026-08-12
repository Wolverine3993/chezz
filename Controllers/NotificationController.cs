using Chezz.Database.EntityManagers;
using Chezz.Database.Models;
using Chezz.RequestSchemas.Notifications;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace Chezz.Controllers
{
    [Route("/api/notificationList")]
    public class NotificationController(UserManager<ChezzUser> _userManager, NotificationManager _notificationManager) : ControllerBase
    {

        [HttpGet]
        public async Task<Results<Ok, Ok<Notification>, UnauthorizedHttpResult>> GetNotifications()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return TypedResults.Unauthorized();
            }
            
            var earliestNotification = await _notificationManager.GetNotificationAsync(user);
            if (earliestNotification is null) return TypedResults.Ok();

            return TypedResults.Ok(earliestNotification);
        }

        [HttpPost("send")]
        public async Task<Results<Ok, UnauthorizedHttpResult>> SendNotification([FromBody] NotificationRequest request)
        {
            var newNotification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                Content = request.Content,
                Title = "Test Title",
                UserId = request.UserId,
            };

            await _notificationManager.AddNotificationAsync(newNotification);

            return TypedResults.Ok();
        }
    }
}
