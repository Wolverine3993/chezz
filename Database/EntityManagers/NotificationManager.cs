using Chezz.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Chezz.Database.EntityManagers
{
    public class NotificationManager(ChezzDbContext _dbContext)
    {
        async Task AddNotificationAsync(Notification notification)
        {
            await _dbContext.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddFriendNotificationAsync(ChezzUser userFrom, ChezzUser userTo, FriendRequest request)
        {
            var newNotification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userTo.Id,
                Title = "Friend Request",
                Content = $"You have recieved a friend request from {userFrom.UserName}.",
                CallbackId = request.Id,
                NotificationType = NotificationType.FriendRequest,
            };

            await AddNotificationAsync(newNotification);
        }

        public async Task AddMatchmakeNotificationAsync(ChezzUser userFrom, ChezzUser userTo, string matchId)
        {
            var newNotification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userTo.Id,
                Title = "Match Request",
                Content = $"You have recieved a match request from {userFrom.UserName}.",
                CallbackId = matchId,
                NotificationType = NotificationType.MatchInvitation,
            };

            await AddNotificationAsync(newNotification);
        }

        public async Task<Notification?> GetNotificationAsync(ChezzUser user)
        {
            var notification = await _dbContext.Notifications
                .Where(notification => notification.UserId == user.Id)
                .FirstOrDefaultAsync();

            if (notification is not null)
            {
                _dbContext.Remove(notification);
                await _dbContext.SaveChangesAsync();
            }

            return notification;
        }

        public async Task RemoveFriendRequestNotificationAsync(ChezzUser userFrom, ChezzUser userTo)
        {
            var notification = await _dbContext.Notifications
                .Where(notification => notification.UserId == userTo.Id && notification.Content == $"You have recieved a friend request from {userFrom.UserName}.")
                .FirstOrDefaultAsync();

            if (notification is not null)
            {
                _dbContext.Remove(notification);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
