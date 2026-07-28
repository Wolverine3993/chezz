using System.Diagnostics;
using Chezz.Game.Board;
using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game.Games.Chess;

public class ChessBoardState : IBoardState, IGameStore<ChessPiece>
{
    private List<IPlayer> _players = new();
    public ChessPiece?[,] Board { get; private set; }

    private static ChessPiece?[,] LoadFen(string fen)
    {
        ChessPiece?[,] board = new ChessPiece?[8,8];
        int x = 0;
        int y = 0;
        foreach (string line in fen.Split("/"))
        {
            foreach (char pieceChar in line)
            {
                string piece = pieceChar.ToString();
                if (int.TryParse(piece, out int skip))
                {
                    x += skip;
                    continue;
                }
                int player = piece.ToUpper() == piece ? 0 : 1; // White if uppercase, otherwise lowercase

                string pieceLowercase = piece.ToLower();
                ChessPiece.ChessPieceEnum type = pieceLowercase switch
                {
                    "p" => ChessPiece.ChessPieceEnum.Pawn,
                    "r" => ChessPiece.ChessPieceEnum.Rook,
                    "n" => ChessPiece.ChessPieceEnum.Knight,
                    "k" => ChessPiece.ChessPieceEnum.King,
                    "q" => ChessPiece.ChessPieceEnum.Queen,
                    "b" => ChessPiece.ChessPieceEnum.Bishop,
                };

                board[x, y] = new ChessPiece(type, player);
            }

            y += 1;
            x = 0;
        }

        return board;
    }

    public void Init()
    {
        Board = LoadFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
    }

    public void AddPlayer(IPlayer player)
    {
        _players.Add(player);
    }

    public void AddMoveByPlayer(IMove move, IPlayer player)
    {
        throw new NotImplementedException();
    }
}