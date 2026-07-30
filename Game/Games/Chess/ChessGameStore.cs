using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game.Games.Chess;

public class ChessGameStore : IGameState<ChessPiece, ChessMove>
{
	public Lobby Lobby { get; set; }
	public ChessPiece?[,] Board { get; private set; }

	private List<(IPlayer, ChessMove)> moveList = new();

	public ChessMove? LastMove => moveList.Count > 0 ? moveList[^1].Item2 : null;

	private ChessPiece?[,] LoadFen(string fen)
	{
		ChessPiece?[,] board = new ChessPiece?[8, 8];
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
					_ => throw new NotImplementedException(),
				};

				board[x, y] = new ChessPiece(type, this.Lobby.Players[player].Id, (ChessPiece.PieceColor)player);
				x += 1;
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

	public bool AddMoveByPlayer(ChessMove move, IPlayer player)
	{
		ChessPiece? from = Board[move.From.X, move.From.Y];
		if (from == null) return false;

		// En passant: a pawn moving diagonally onto an empty square captures the
		// enemy pawn sitting on the moving pawn's origin rank.
		if (from.Type == ChessPiece.ChessPieceEnum.Pawn
			&& move.From.X != move.To.X
			&& Board[move.To.X, move.To.Y] == null)
		{
			Board[move.To.X, move.From.Y] = null;
		}

		Board[move.From.X, move.From.Y] = null;
		Board[move.To.X, move.To.Y] = from;

		moveList.Add((player, move));

		return true;
	}
}