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
        NotificationManager _notificationManager,
        UserRelationshipManager _userRelationshipManager,
        UserManager<ChezzUser> _userManager) : ControllerBase
    {
        [HttpPost("add-friend")]
        public async Task<Results<Ok, Conflict<string>, UnauthorizedHttpResult, NotFound>> AddFriendRequest([FromBody] RelationshipRequest request)
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
                return TypedResults.Conflict("You cannot be friends with yourself.");
            }

            if (await _userRelationshipManager.AreFriendsAsync(user1, user2))
            {
                return TypedResults.Conflict("You are already friends.");
            }

            if (await _userRelationshipManager.FriendRequestExistsAsync(user2, user1))
            {
                await _userRelationshipManager.AddUserRelationshipAsync(user1, user2);
                await _userRelationshipManager.AddUserRelationshipAsync(user2, user1);

                await _userRelationshipManager.RemoveFriendRequestAsync(user2, user1);

                return TypedResults.Ok();
            }

            if (await _userRelationshipManager.AddFriendRequestAsync(user1, user2))
            {
                await _notificationManager.AddFriendNotificationAsync(user1, user2);
            }

            return TypedResults.Ok();
        }

        [HttpGet("get-friends")]
        public async Task<Results<Ok<IEnumerable<FriendResponse>>, UnauthorizedHttpResult>> GetFriends()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            var relationships = await _userRelationshipManager.GetUserRelationshipsAsync(user);
            var friends = relationships.Select(relationship => new FriendResponse
            {
                Username = relationship.User2.UserName,
                Id = relationship.User2.Id
            });

            return TypedResults.Ok(friends);
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

        [HttpGet("get-friend-requests")]
        public async Task<Results<Ok<IEnumerable<FriendRequest>>, UnauthorizedHttpResult>> GetFriendRequests()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            return TypedResults.Ok(await _userRelationshipManager.GetFriendRequestsAsync(user));
        }
    }
}
