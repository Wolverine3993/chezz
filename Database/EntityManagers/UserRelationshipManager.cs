using Chezz.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace Chezz.Database.EntityManagers
{
    public class UserRelationshipManager(ChezzDbContext _dbContext)
    {
        async Task AddUserRelationshipAsync(ChezzUser user1, ChezzUser user2)
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
                // ...
            }
        }

        public async Task MakeFriendsAsync(ChezzUser user1, ChezzUser user2)
        {
            await AddUserRelationshipAsync(user1, user2);
            await AddUserRelationshipAsync(user2, user1);
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
    }
}
