using Chezz.Database.EntityManagers;
using Chezz.Database.Models;
using Chezz.Game;
using Chezz.Game.Games.Chess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chezz.Controllers
{
	[ApiController]
	[Route("/api/games/chess")]
	public class ChessController : GameController<ChessMove, ChessPiece, ChessGameState, ChessGameStore, ChessGameImplementation>
	{
		public ChessController(UserManager<ChezzUser> userManager, LobbyRegistry lobbyRegistry, GameRegistry<ChessMove, ChessPiece, ChessGameState, ChessGameStore, ChessGameImplementation> registry, UserRelationshipManager userRelationshipManager, NotificationManager notificationManager) : base(userManager, notificationManager, userRelationshipManager, lobbyRegistry, registry)
		{
		}

		public override GameType GameType => GameType.Chess;

		public override int MaxPlayers => 2;
	}
}
