using Chezz.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Chezz.Database.EntityManagers
{
    public class FriendRequestManager(ChezzDbContext _dbContext)
    {
        public async Task<FriendRequest?> GetFriendRequestByIdAsync(string requestId)
        {
            return await _dbContext.FriendRequests
                .Include(request => request.UserFrom)
                .Include(request => request.UserTo)
                .FirstOrDefaultAsync(request => request.Id == requestId);
        }
        public async Task<IEnumerable<FriendRequest>> GetFriendRequestsAsync(ChezzUser user)
        {
            return await _dbContext.FriendRequests
                .Include(request => request.UserTo)
                .Include(request => request.UserFrom)
                .Where(request => request.UserTo == user)
                .ToListAsync();
        }

        public async Task<bool> FriendRequestExistsAsync(ChezzUser user1, ChezzUser user2)
        {
            return await _dbContext.FriendRequests
                .Where(request => request.UserFromId == user1.Id
                    && request.UserToId == user2.Id)
                .FirstOrDefaultAsync() is not null;
        }

        public async Task<FriendRequest?> AddFriendRequestAsync(ChezzUser userFrom, ChezzUser userTo)
        {
            var newRequest = new FriendRequest
            {
                Id = Guid.NewGuid().ToString(),
                UserFrom = userFrom,
                UserTo = userTo,
            };

            try
            {
                await _dbContext.FriendRequests.AddAsync(newRequest);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return null;
            }

            return newRequest;
        }

        public async Task RemoveFriendRequestAsync(ChezzUser userFrom, ChezzUser userTo)
        {
            var requestToRemove = await _dbContext.FriendRequests
                .Where(request => request.UserFromId == request.UserFromId
                    && request.UserToId == request.UserToId)
                .FirstOrDefaultAsync();

            if (requestToRemove is null)
            {
                return;
            }

            _dbContext.Remove(requestToRemove);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFriendRequestAsync(string requestId)
        {
            var requestToRemove = await _dbContext.FriendRequests
                .Where(request => request.Id == requestId)
                .FirstOrDefaultAsync();

            if (requestToRemove is null)
            {
                return;
            }

            _dbContext.Remove(requestToRemove);
            await _dbContext.SaveChangesAsync();
        }
    }
}
