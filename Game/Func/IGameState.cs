using Chezz.Game.Players;

namespace Chezz.Game.Func;

public interface IGameState<TPiece, TMove> where TPiece : IPiece where TMove : IMove
{
	void Init();
	void AddPlayer(IPlayer player);
	bool AddMoveByPlayer(TMove move, IPlayer player);
	TPiece?[,] Board { get; }
}