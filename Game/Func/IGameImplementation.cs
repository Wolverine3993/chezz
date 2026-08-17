using Chezz.Game.Players;

namespace Chezz.Game.Func;

public interface IGameImplementation<TPiece, TMove, TGameSerializedState, TGameStore> where TPiece : IPiece where TMove : IMove where TGameStore : IGameState<TMove>
{
	public int CurrentTurn { get; set; }

	List<TMove> GetValidMoves(TGameStore gameStore, IPlayer player);

	void OnMakeMove(TGameStore gameStore, IPlayer player, TMove move);

	TGameSerializedState GetSerializedState(TGameStore gameStore, IPlayer player);
}