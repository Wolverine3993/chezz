using Chezz.Database.EntityManagers;
using Chezz.Database.Models;
using Chezz.RequestSchemas.UserRelationships;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Chezz.Controllers
{
    [Route("/api/relationship")]
    public class UserRelationshipController(
        UserRelationshipManager _userRelationshipManager,
        UserManager<ChezzUser> _userManager) : ControllerBase
    {
        [HttpPost("add-friend")]
        public async Task<Results<Ok, Conflict<Dictionary<string, string>>, UnauthorizedHttpResult, NotFound>> CreateRelationship([FromBody] RelationshipRequest request)
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
                return TypedResults.Conflict(new Dictionary<string, string>
                {
                    { "reason", "You cannot be friends with yourself" }
                });
            }

            if (!await _userRelationshipManager.AddUserRelationshipAsync(user1, user2))
            {
                return TypedResults.Conflict(new Dictionary<string, string>
                {
                    { "reason", "This relationship already exists" }
                });
            }

            return TypedResults.Ok();
        }

        [HttpGet("get-friends")]
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

        [HttpDelete("remove-friend")]
        public async Task<Results<Ok, UnauthorizedHttpResult, NotFound<Dictionary<string, string>>>> RemoveFriend([FromBody] RelationshipRequest request)
        {
            var user1 = await _userManager.GetUserAsync(HttpContext.User);
            if (user1 is null)
            {
                return TypedResults.Unauthorized();
            }

            var user2 = await _userManager.FindByNameAsync(request.Username);
            if (user2 is null)
            {
                return TypedResults.NotFound(new Dictionary<string, string>
                {
                    { "reason", $"Cannot find user with username {request.Username}"}
                });
            }

            await _userRelationshipManager.RemoveUserRelationshipAsync(user1, user2);
            return TypedResults.Ok();
        }
    }
}
