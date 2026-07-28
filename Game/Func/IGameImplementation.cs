using Chezz.Game.Players;

namespace Chezz.Game.Func;

public interface IGameImplementation<TPiece, TGs> where TPiece : IPiece where TGs: IGameStore<TPiece>
{
    List<IMove> GetValidMoves(TGs gameStore, IPlayer player);
}