using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game.Games.Chess;

public class ChessGameImplementation : IGameImplementation<ChessPiece, ChessBoardState>
{
    public List<IMove> GetValidMoves(ChessBoardState gameStore, IPlayer player)
    {
        throw new NotImplementedException();
    }
}