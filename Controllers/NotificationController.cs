using Chezz.Database.EntityManagers;
using Chezz.Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chezz.Controllers
{
    [Route("/api/notificationList")]
    public class NotificationController(UserManager<ChezzUser> _userManager, NotificationManager _notificationManager) : ControllerBase
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
    }
}
