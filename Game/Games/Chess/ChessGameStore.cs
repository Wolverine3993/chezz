using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game.Games.Chess;

public class ChessGameStore : IGameState<ChessPiece, ChessMove>
{
	public Lobby Lobby { get; set; }
	public ChessPiece?[,] Board { get; private set; }

	private List<(IPlayer, ChessMove)> moveList = new();

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
		ChessPiece? from = Board[move.from.Item1, move.from.Item2];
		if (from == null) return false;

		Board[move.from.Item1, move.from.Item2] = null;
		Board[move.to.Item1, move.to.Item2] = from;

		moveList.Add((player, move));

		return true;
	}
}