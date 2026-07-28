using Chezz.Game.Players;

namespace Chezz.Game.Func;

public interface IGameStore<TPiece> where TPiece : IPiece
{
    void Init();
    void AddPlayer(IPlayer player);
    void AddMoveByPlayer(IMove move, IPlayer player);
    TPiece?[,] Board { get; }
}