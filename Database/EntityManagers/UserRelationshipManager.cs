using Chezz.Database.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<UserRelationship>> GetUserRelationshipsAsync(ChezzUser user, int page)
        {
            return await _dbContext.UserRelationships
                .Include(relationship => relationship.User1)
                .Include(relationship => relationship.User2)
                .Where(relationship => relationship.User1 == user)
                .AsAsyncEnumerable()
                .Take(new Range(new Index(20 * page), new Index(19 + 20 * page)))
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
