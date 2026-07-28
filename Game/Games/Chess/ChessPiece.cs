using Chezz.Game.Func;

namespace Chezz.Game.Games.Chess;

public class ChessPiece : IPiece
{
    public enum ChessPieceEnum
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King,
    }

    private ChessPieceEnum type;
    private int player;

    public ChessPiece(ChessPieceEnum type, int player)
    {
        this.type = type;
        this.player = player;
    }
    
    public string ImageUrl()
    {
        throw new NotImplementedException();
    }
}