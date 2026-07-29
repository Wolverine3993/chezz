using Chezz.Game.Players;

namespace Chezz.Game.Func;

public interface IGameState<TPiece, TMove> where TPiece : IPiece where TMove : IMove
{
	public Lobby Lobby { get; set; }

	void Init();
	bool AddMoveByPlayer(TMove move, IPlayer player);
	TPiece?[,] Board { get; }
}