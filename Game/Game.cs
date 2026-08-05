using Chezz.Game.Board;
using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game;

public class Game<TMove, TPiece, TGameStatus, TGameState, TGameImplementation> : IBoardStyle where TPiece : IPiece
	where TMove : IMove
	where TGameState : IGameState<TMove>
	where TGameImplementation : IGameImplementation<TPiece, TMove, TGameStatus, TGameState>
{
	public Guid Id { get; }
	public TGameState GameStore { get; }
	public TGameImplementation GameImplementation { get; }

	private List<TMove>? currentMoves;

	private async Task Notify()
	{
		await Task.WhenAll(GameStore.Lobby.Players.Select(v => v.Notify()));
	}

	public Game(Lobby lobby, TGameState gameStore, TGameImplementation gameImplementation)
	{
		Id = Guid.NewGuid();
		GameStore = gameStore;
		GameImplementation = gameImplementation;

		GameStore.Lobby = lobby;
		lobby.GameId = Id;

		GameStore.Init();

		var _ = Notify();
	}

	public List<TMove> RequestMovesForPlayer(IPlayer player)
	{
		if (!IsTurn(player)) return new();
		if (currentMoves != null)
		{
			return currentMoves;
		}
		currentMoves = GameImplementation.GetValidMoves(GameStore, GameStore.Lobby.Players[GameImplementation.CurrentTurn]);
		return currentMoves;
	}

	public bool IsTurn(IPlayer player)
	{
		return GameStore.Lobby.Players[GameImplementation.CurrentTurn].Id == player.Id;
	}

	public async Task<bool> MakeMove(IPlayer player, string moveId)
	{
		if (!IsTurn(player)) return false;
		if (currentMoves == null)
		{
			currentMoves = RequestMovesForPlayer(player);
		}

		TMove? move = currentMoves.Find((v) => v.MoveId == moveId);

		if (move == null) return false;

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