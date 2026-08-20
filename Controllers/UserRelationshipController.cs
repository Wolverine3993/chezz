using Chezz.Database.EntityManagers;
using Chezz.Database.Models;
using Chezz.RequestSchemas.UserRelationships;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Runtime.Intrinsics.X86;

namespace Chezz.Controllers
{
    [Route("/api/relationship")]
    public class UserRelationshipController(
        NotificationManager _notificationManager,
        UserRelationshipManager _userRelationshipManager,
        FriendRequestManager _friendRequestManager,
        UserManager<ChezzUser> _userManager) : ControllerBase
    {
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

        [HttpGet("get-friend-requests")]
        public async Task<Results<Ok<IEnumerable<FriendRequest>>, UnauthorizedHttpResult>> GetFriendRequests()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            return TypedResults.Ok(await _friendRequestManager.GetFriendRequestsAsync(user));
        }

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

            if (await _friendRequestManager.FriendRequestExistsAsync(user2, user1))
            {
                await _userRelationshipManager.MakeFriendsAsync(user1, user2);
                await _friendRequestManager.RemoveFriendRequestAsync(user2, user1);
                await _notificationManager.RemoveFriendRequestNotificationAsync(user2, user1);

                return TypedResults.Ok();
            }

            var friendRequest = await _friendRequestManager.AddFriendRequestAsync(user1, user2);
            if (friendRequest is not null)
            {
                await _notificationManager.AddFriendNotificationAsync(user1, user2, friendRequest);
            }

            return TypedResults.Ok();
        }

        [HttpPost("accept-friend-request")]
        public async Task<Results<Ok, UnauthorizedHttpResult, NotFound>> AcceptFriendRequest([FromBody] FriendAcceptRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            var friendRequest = await _friendRequestManager.GetFriendRequestByIdAsync(request.RequestId);

            if (friendRequest is null)
            {
                return TypedResults.NotFound();
            }

            if (friendRequest.UserFrom == user)
            {
                return TypedResults.Unauthorized();
            }

            await _userRelationshipManager.MakeFriendsAsync(friendRequest.UserFrom, friendRequest.UserTo);
            await _friendRequestManager.RemoveFriendRequestAsync(friendRequest.Id);

            return TypedResults.Ok();
        }

        [HttpPost("decline-friend-request")]
        public async Task<Results<Ok, UnauthorizedHttpResult>> DeclineFriendRequest([FromBody] FriendDeclineRequest request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            await _friendRequestManager.RemoveFriendRequestAsync(request.RequestId);

            return TypedResults.Ok();
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
