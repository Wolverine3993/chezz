using Chezz.Game.Func;
using Chezz.Game.Players;
using System.Text;

namespace Chezz.Game.Games.Chess;

public class ChessGameStore : IGameState<ChessMove>
{
	public Lobby Lobby { get; set; }
	public ChessPiece?[,] Board { get; private set; }

	private List<(IPlayer, ChessMove)> moveList = new();

	public ChessMove? LastMove => moveList.Count > 0 ? moveList[^1].Item2 : null;

	public List<string> MoveHistory => moveList.Select(entry => FormatMove(entry.Item2)).ToList();

	private static string FormatMove(ChessMove move)
	{
		if (move is CastlingChessMove)
		{
			return move.To.X > move.From.X ? "O-O" : "O-O-O";
		}

		string notation = $"{SquareName(move.From)}{SquareName(move.To)}";
		if (move is PromotionChessMove promotion)
		{
			notation += $"={PieceLetter(promotion.PromotionPiece.Type)}";
		}

		return notation;
	}

	private static string SquareName(ChessPosition position)
		=> $"{(char)('a' + position.X)}{8 - position.Y}";

	private static string PieceLetter(ChessPiece.ChessPieceEnum type) => type switch
	{
		ChessPiece.ChessPieceEnum.Queen => "Q",
		ChessPiece.ChessPieceEnum.Rook => "R",
		ChessPiece.ChessPieceEnum.Bishop => "B",
		ChessPiece.ChessPieceEnum.Knight => "N",
		_ => "",
	};

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
				int player = piece.ToUpper() == piece ? 0 : 1;

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

	public PackedBoardState ToPackedBoard()
	{
		StringBuilder builder = new StringBuilder();
		Dictionary<string, string> imageUrls = new();
		for (int x = 0; x < Board.GetLength(0); x++)
		{
			for (int y = 0; y < Board.GetLength(1); y++)
			{
				var piece = Board[x, y];
				if (piece == null)
				{
					builder.Append(".");
					continue;
				}

				var capital = piece.Color == ChessPiece.PieceColor.White;
				var pieceChar = piece.Type switch
				{
					ChessPiece.ChessPieceEnum.Pawn => "p",
					ChessPiece.ChessPieceEnum.Knight => "n",
					ChessPiece.ChessPieceEnum.Bishop => "b",
					ChessPiece.ChessPieceEnum.Rook => "r",
					ChessPiece.ChessPieceEnum.Queen => "q",
					ChessPiece.ChessPieceEnum.King => "k",
				};
				pieceChar = capital ? pieceChar.ToUpper() : pieceChar.ToLower();

				builder.Append(pieceChar);
				imageUrls.TryAdd(pieceChar, piece.ImageUrl);
			}
			builder.Append("\n");
		}
		string packedBoard = builder.ToString();

		return new() { PackedBoard = packedBoard, ImageUrls = imageUrls }; ;
	}
}