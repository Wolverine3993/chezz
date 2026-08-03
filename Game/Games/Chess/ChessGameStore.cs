using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game.Games.Chess;

public class ChessGameStore : IGameState<ChessPiece, ChessMove>
{
	public Lobby Lobby { get; set; }
	public ChessPiece?[,] Board { get; private set; }

	private List<(IPlayer, ChessMove)> moveList = new();

	public ChessMove? LastMove => moveList.Count > 0 ? moveList[^1].Item2 : null;

	public bool HasMovedFrom(ChessPosition position)
	{
		foreach ((IPlayer _, ChessMove move) in moveList)
		{
			if (move.From == position) return true;
		}

		return false;
	}

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
				int player = piece.ToUpper() == piece ? 0 : 1; // White if uppercase

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
		bool result = move.MutateBoard(this.Board);
		if (!result) return false;

		moveList.Add((player, move));

		return true;
	}
}