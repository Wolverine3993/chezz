using Chezz.Database.Models;
using Chezz.Errors;
using Chezz.Game;
using Chezz.Game.Func;
using Chezz.Game.Players;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chezz.Controllers;

[Authorize]
public abstract class GameController<TMove, TPiece, TGameStatus, TGs, TGi> : ControllerBase where TPiece : IPiece
	where TMove : IMove
	where TGs : IGameState<TPiece, TMove>, new()
	where TGi : IGameImplementation<TPiece, TMove, TGameStatus, TGs>, new()
{
	protected GameRegistry<TMove, TPiece, TGameStatus, TGs, TGi> GameRegistry { get; }
	protected LobbyRegistry LobbyRegistry { get; }
	protected UserManager<ChezzUser> UserManager;

	protected GameController(
		UserManager<ChezzUser> userManager,
		LobbyRegistry lobbyRegistry,
		GameRegistry<TMove, TPiece, TGameStatus, TGs, TGi> registry)
	{
		UserManager = userManager;
		LobbyRegistry = lobbyRegistry;
		GameRegistry = registry;
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
	public async Task<Guid> CreateLobby()
	{
		Lobby lobby = new Lobby(MaxPlayers, GameType);
		LobbyRegistry.Add(lobby);
		return lobby.Id;
	}

	[Route("lobby/{lobbyId}/ws")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task JoinLobby(Guid lobbyId)
	{
		if (!HttpContext.WebSockets.IsWebSocketRequest) throw new BadRequestException("Websocket-only endpoint");

		var (lobby, user) = await GetLobbyUser(lobbyId);

		using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
		WebsocketPlayer player = new WebsocketPlayer(webSocket, user);
		lobby.AddPlayer(user, player);

		if (lobby.CanStartGame())
		{
			GameRegistry.Add(new(lobby, new TGs(), new TGi()));
		}
	}

	[HttpGet("lobby/{lobbyId}/status", Name = "GetLobbyStatus")]
	public async Task<Lobby.LobbyInformation> GetLobbyStatus(Guid lobbyId)
	{
		var (lobby, user) = await GetLobbyUser(lobbyId);

		return lobby.GetLobbyInformation();
	}

	[HttpGet("game/{gameId}/status", Name = "GetGameStatus")]
	public async Task<TGameStatus> GetGameStatus(Guid gameId)
	{
		var (game, user) = await GetGameUser(gameId);

		var player = game.GameImplementation.Lobby.FromChezzUser(user);
		if (player == null) throw new BadRequestException("Not in game, or cannot convert to IPlayer for some other reason");

		return game.GetStatus(player);
	}
}