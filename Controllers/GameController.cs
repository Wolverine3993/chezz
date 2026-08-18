using Chezz.Database.EntityManagers;
using Chezz.Database.Models;
using Chezz.Errors;
using Chezz.Game;
using Chezz.Game.Func;
using Chezz.Game.Players;
using Chezz.RequestSchemas.UserRelationships;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chezz.Controllers;

[Authorize]
public abstract class GameController<TMove, TPiece, TGameStatus, TGs, TGi> : ControllerBase where TPiece : IPiece
	where TMove : IMove
	where TGs : IGameState<TMove>, new()
	where TGi : IGameImplementation<TPiece, TMove, TGameStatus, TGs>, new()
{
	protected GameRegistry<TMove, TPiece, TGameStatus, TGs, TGi> GameRegistry { get; }
	protected LobbyRegistry LobbyRegistry { get; }
	protected UserManager<ChezzUser> UserManager;
	protected NotificationManager NotificationManager;
	protected UserRelationshipManager UserRelationshipManager;


    protected GameController(
		UserManager<ChezzUser> userManager,
		NotificationManager notificationManager,
		UserRelationshipManager userRelationshipManager,
		LobbyRegistry lobbyRegistry,
		GameRegistry<TMove, TPiece, TGameStatus, TGs, TGi> registry)
	{
		UserManager = userManager;
		LobbyRegistry = lobbyRegistry;
		GameRegistry = registry;
		NotificationManager = notificationManager;
		UserRelationshipManager = userRelationshipManager;
	}

	public abstract GameType GameType { get; }
	public abstract int MaxPlayers { get; }

	private async Task<(Lobby, ChezzUser)> GetLobbyUser(Guid lobbyId)
	{
		Lobby? lobby = LobbyRegistry.Get(GameType, lobbyId);
		if (lobby == null) throw new NotFoundException("Lobby not found");

		ChezzUser? user = await UserManager.GetUserAsync(HttpContext.User);
		if (user == null) throw new ChezzError(StatusCodes.Status401Unauthorized, "User is null");

        return (lobby, user);
	}

	private async Task<(Game<TMove, TPiece, TGameStatus, TGs, TGi>, ChezzUser)> GetGameUser(Guid gameId)
	{
		var game = GameRegistry.Get(gameId);
		if (game == null) throw new NotFoundException("Game not found");

		ChezzUser? user = await UserManager.GetUserAsync(HttpContext.User);
		if (user == null) throw new ChezzError(StatusCodes.Status401Unauthorized, "User is null");

		return (game, user);
	}


	[HttpPost("lobby/create", Name = "CreateLobby")]
	public async Task<Guid> CreateLobby([FromQuery] bool isPrivate = true)
	{
		Lobby lobby = new Lobby(MaxPlayers, GameType, isPrivate);
		LobbyRegistry.Add(lobby);
		return lobby.Id;
	}

    [HttpPost("lobby/matchmake", Name = "Matchmake")]
    public async Task<Guid> Matchmake()
    {
        var openLobby = LobbyRegistry.All(GameType)
			.Where(lobby => !lobby.IsPrivate)
			.OrderByDescending(lobby => lobby.Players.Count)
            .FirstOrDefault(lobby => lobby.Players.Count < MaxPlayers);

        var lobbyId = openLobby?.Id ?? await CreateLobby(false);
        return lobbyId;
    }

	[HttpPost("lobby/send-request", Name = "SendRequest")]
	public async Task<Results<Ok<Guid>, UnauthorizedHttpResult, NotFound>> SendRequest([FromBody] RelationshipRequest request)
	{
		var lobbyId = await CreateLobby();
        var user = await UserManager.GetUserAsync(HttpContext.User);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var userToMatchmake = await UserManager.FindByNameAsync(request.Username);

        if (userToMatchmake is null)
        {
            return TypedResults.NotFound();
        }

        if (!await UserRelationshipManager.AreFriendsAsync(user, userToMatchmake))
        {
            return TypedResults.Unauthorized();
        }

		await NotificationManager.AddMatchmakeNotificationAsync(user, userToMatchmake, lobbyId.ToString());
		return TypedResults.Ok(lobbyId);
    }

    [Route("lobby/{lobbyId}/ws")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task JoinLobby(Guid lobbyId)
	{
		if (!HttpContext.WebSockets.IsWebSocketRequest) throw new BadRequestException("Websocket-only endpoint");

		Lobby lobby;
		ChezzUser user;
		try
		{
			(lobby, user) = await GetLobbyUser(lobbyId);
		}
		catch (ChezzError ex) when (!HttpContext.Response.HasStarted)
		{
			HttpContext.Response.StatusCode = ex.StatusCode;
			return;
		}

		using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
		WebsocketPlayer player = new WebsocketPlayer(webSocket, user);
		lobby.AddPlayer(player, user);

		if (lobby.CanStartGame())
		{
			GameRegistry.Add(new(lobby, new TGs(), new TGi()));
		}

		await player.WaitForCloseAsync();
	}

	[HttpGet("lobby/{lobbyId}/status", Name = "GetLobbyStatus")]
	public async Task<Lobby.LobbyInformation> GetLobbyStatus(Guid lobbyId)
	{
		var (lobby, user) = await GetLobbyUser(lobbyId);

		return lobby.GetLobbyInformation();
	}

	[HttpPost("lobby/{lobbyId}/privacy", Name = "ChangeLobbyPrivacy")]
	public async Task ChangeLobbyPrivacy(Guid lobbyId, [FromQuery] bool isPrivate)
	{
		var (lobby, user) = await GetLobbyUser(lobbyId);
		if (!lobby.Players.Any(player => player.UserId == user.Id)) throw new ChezzError(StatusCodes.Status401Unauthorized, "User is not in lobby");

        lobby.ChangePrivacy(isPrivate);
	}

    [HttpGet("lobby/{lobbyId}/privacy", Name = "GetLobbyPrivacy")]
    public async Task<bool> GetLobbyPrivacy(Guid lobbyId)
    {
        var (lobby, user) = await GetLobbyUser(lobbyId);
		return lobby.IsPrivate;
    }

    [HttpGet("game/{gameId}/status", Name = "GetGameStatus")]
	public async Task<TGameStatus> GetGameStatus(Guid gameId)
	{
		var (game, user) = await GetGameUser(gameId);

		var player = game.GameStore.Lobby.FromChezzUser(user);
		if (player == null) throw new BadRequestException("Not in game, or cannot convert to IPlayer for some other reason");

		return game.GetStatus(player);
	}

	[HttpGet("game/{gameId}/moves", Name = "GetMoves")]
	public async Task<List<TMove>> GetMoves(Guid gameId)
	{
		var (game, user) = await GetGameUser(gameId);

		var player = game.GameStore.Lobby.FromChezzUser(user);
		if (player == null) throw new BadRequestException("Not in game, or cannot convert IPlayer");

		return game.RequestMovesForPlayer(player);
	}

	public record MoveRequest(string moveId);

	[HttpPost("game/{gameId}/move", Name = "MakeMove")]
	public async Task<bool> MakeMove(Guid gameId, [FromBody] MoveRequest move)
	{
		var (game, user) = await GetGameUser(gameId);

		var player = game.GameStore.Lobby.FromChezzUser(user);
		if (player == null) throw new BadRequestException("Not in game, or cannot convert IPlayer");

		return await game.MakeMove(player, move.moveId);
	}

	[HttpPost("lobby/{lobbyId}/add-bot", Name = "AddBot")]
	public async Task<Guid> AddBot(Guid lobbyId)
	{
		var (lobby, user) = await GetLobbyUser(lobbyId);
		if (!lobby.Players.Any(player => player.UserId == user.Id))
			throw new ChezzError(StatusCodes.Status401Unauthorized, "User is not in lobby");
		if (lobby.GameId != null) throw new BadRequestException("Game already started");

		var bot = new BotPlayer<TMove, TPiece, TGameStatus, TGs, TGi>();
		if (!lobby.AddPlayer(bot)) throw new BadRequestException("Lobby is full");

		if (lobby.CanStartGame())
		{
			var game = new Game<TMove, TPiece, TGameStatus, TGs, TGi>(lobby, new TGs(), new TGi());
			GameRegistry.Add(game);
			bot.AttachGame(game);
		}

		return lobby.Id;
	}
}