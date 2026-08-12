using Chezz.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Chezz.Database.EntityManagers
{
    public class NotificationManager(ChezzDbContext _dbContext)
    {
        public async Task AddNotificationAsync(Notification notification)
        {
            await _dbContext.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
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
    }
}
