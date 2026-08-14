using Chezz.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace Chezz.Database.EntityManagers
{
    public class UserRelationshipManager(ChezzDbContext _dbContext)
    {
        public async Task<bool> AddUserRelationshipAsync(ChezzUser user1, ChezzUser user2)
        {
            var newRelationship = new UserRelationship
            {
                Id = Guid.NewGuid().ToString(),
                User1 = user1,
                User2 = user2,
            };

            try
            {
                await _dbContext.UserRelationships.AddAsync(newRelationship);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> AreFriendsAsync(ChezzUser user1, ChezzUser user2)
        {
            return await _dbContext.UserRelationships
                .AnyAsync(relationship => relationship.User1Id == user1.Id
                        && relationship.User2Id == user2.Id);
        }

        public async Task RemoveUserRelationshipAsync(ChezzUser user1, ChezzUser user2)
        {
            var targetRelationship = await _dbContext.UserRelationships
                .Include(relationship => relationship.User1)
                .Include(relationship => relationship.User2)
                .Where(relationship => (relationship.User1 == user1 && relationship.User2 == user2)
                                    || (relationship.User1 == user2 && relationship.User2 == user1))
                .ToArrayAsync();
            
            if (targetRelationship is not null)
            {
                _dbContext.UserRelationships.RemoveRange(targetRelationship);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserRelationship>> GetUserRelationshipsAsync(ChezzUser user)
        {
            return await _dbContext.UserRelationships
                .Include(relationship => relationship.User1)
                .Include(relationship => relationship.User2)
                .Where(relationship => relationship.User1 == user)
                .ToListAsync();
        }

        public async Task<int> GetFriendCountAsync(ChezzUser user)
        {
            return await _dbContext.UserRelationships
                .Include(relationship => relationship.User1)
                .Where(relationship => relationship.User1 == user)
                .CountAsync();
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

        public async Task<bool> AddFriendRequestAsync(ChezzUser userFrom, ChezzUser userTo)
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
                return false;
            }

            return true;
        }

        public async Task RemoveFriendRequestAsync(ChezzUser userFrom, ChezzUser userTo)
        {
            var requestToRemove = _dbContext.FriendRequests
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
    }
}
