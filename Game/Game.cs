using Chezz.Game.Board;
using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game;

public class Game<TMove, TPiece, TGameStatus, TGameState, TGameImplementation> : IBoardStyle where TPiece : IPiece
	where TMove : IMove
	where TGameState : IGameState<TPiece, TMove>
	where TGameImplementation : IGameImplementation<TPiece, TMove, TGameStatus, TGameState>
{
	public Guid Id { get; }
	public TGameState GameStore { get; }
	public TGameImplementation GameImplementation { get; }

	private List<TMove>? currentMoves;

	private async Task Notify()
	{
		await Task.WhenAll(GameImplementation.Lobby.Players.Select(v => v.Notify()));
	}

	public Game(Lobby lobby, TGameState gameStore, TGameImplementation gameImplementation)
	{
		Id = Guid.NewGuid();
		GameStore = gameStore;
		GameImplementation = gameImplementation;

		GameImplementation.Lobby = lobby;
		lobby.GameId = Id;

		var _ = Notify();
	}

	public List<TMove> RequestMovesForPlayer(IPlayer player)
	{
		if (!IsTurn(player)) return new();
		if (currentMoves != null)
		{
			return currentMoves;
		}
		currentMoves = GameImplementation.GetValidMoves(GameStore, GameImplementation.Lobby.Players[GameImplementation.CurrentTurn]);
		return currentMoves;
	}

	public bool IsTurn(IPlayer player)
	{
		return GameImplementation.Lobby.Players[GameImplementation.CurrentTurn].Id == player.Id;
	}

	public async Task<bool> MakeMove(IPlayer player, TMove move)
	{
		if (!IsTurn(player)) return false;
		if (currentMoves == null)
		{
			currentMoves = RequestMovesForPlayer(player);
		}

		if (!currentMoves.Contains(move)) return false;

		bool result = GameStore.AddMoveByPlayer(move, player);
		if (!result) return false;

		// Increment turn
		GameImplementation.OnMakeMove(GameStore, player, move);
		currentMoves = null;
		await Notify();
		return true;
	}

	public TGameStatus GetStatus(IPlayer player)
	{
		return GameImplementation.GetStatus(GameStore, player);
	}
}