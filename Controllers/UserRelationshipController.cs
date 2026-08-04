using Chezz.Database.EntityManagers;
using Chezz.Database.Models;
using Chezz.RequestSchemas.UserRelationships;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chezz.Controllers
{
    [Route("/api/relationship")]
    public class UserRelationshipController(
        UserRelationshipManager _userRelationshipManager,
        UserManager<ChezzUser> _userManager) : ControllerBase
    {
        [HttpPost]
        public async Task<Results<Ok, Conflict<Dictionary<string, string>>, UnauthorizedHttpResult, NotFound>> CreateRelationship([FromBody] CreateRelationshipRequest request)
        {
            var user1 = await _userManager.GetUserAsync(HttpContext.User);
            if (user1 is null)
            {
                return TypedResults.Unauthorized();
            }

            var user2 = await _userManager.FindByNameAsync(request.Username);
            if (user2 is null)
            {
                return TypedResults.NotFound();
            }

            if (user1 == user2)
            {
                var failReason = new Dictionary<string, string>
                {
                    { "reason", "You cannot be friends with yourself" }
                };
                return TypedResults.Conflict(failReason);
            }

            if (!await _userRelationshipManager.AddUserRelationshipAsync(user1, user2))
            {
                var failReason = new Dictionary<string, string>
                {
                    { "reason", "This relationship already exists" }
                };
                return TypedResults.Conflict(failReason);
            }

            return TypedResults.Ok();
        }

        [HttpGet]
        public async Task<Results<Ok<PagedFriendResponse>, UnauthorizedHttpResult>> GetFriends([FromQuery] int page = 0)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            var relationships = await _userRelationshipManager.GetUserRelationshipsAsync(user, page);
            var friends = relationships.Select(relationship => new FriendResponse
            {
                Username = relationship.User2.UserName,
                Id = relationship.User2.Id
            });
            var friendCount = await _userRelationshipManager.GetFriendCountAsync(user);

            var friendResponse = new PagedFriendResponse
            {
                Friends = friends,
                FriendCount = friendCount,
                Page = page,
            };

            return TypedResults.Ok(friendResponse);
        }
    }
}
