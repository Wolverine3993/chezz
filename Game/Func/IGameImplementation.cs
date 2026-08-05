using Chezz.Game.Players;

namespace Chezz.Game.Func;

public interface IGameImplementation<TPiece, TMove, TGameStatus, TGameStore> where TPiece : IPiece where TMove : IMove where TGameStore : IGameState<TMove>
{
	public int CurrentTurn { get; set; }

	List<TMove> GetValidMoves(TGameStore gameStore, IPlayer player);

	void OnMakeMove(TGameStore gameStore, IPlayer player, TMove move);

	TGameStatus GetStatus(TGameStore gameStore, IPlayer player);
}