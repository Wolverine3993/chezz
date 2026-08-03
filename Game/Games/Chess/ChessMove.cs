using Chezz.Game.Func;
using System.Text.Json.Serialization;

namespace Chezz.Game.Games.Chess
{
	public readonly record struct ChessPosition(int X, int Y)
	{
		public static implicit operator ChessPosition((int, int) tuple) => new(tuple.Item1, tuple.Item2);
		public static implicit operator (int, int)(ChessPosition position) => (position.X, position.Y);
	}

	// Polymorphic serialization: each subtype is emitted as its own schema under a
	// oneOf, tagged by the "kind" discriminator. This lets subtype-only data (e.g.
	// PromotionChessMove.PromotionPiece) appear only on the variant that has it.
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
	[JsonDerivedType(typeof(NormalChessMove), "Normal")]
	[JsonDerivedType(typeof(EnPassantChessMove), "EnPassant")]
	[JsonDerivedType(typeof(CastlingChessMove), "Castle")]
	[JsonDerivedType(typeof(PromotionChessMove), "Promotion")]
	public abstract class ChessMove : IMove
	{
		public ChessPosition From { get; set; }
		public ChessPosition To { get; set; }

		public abstract string MoveId { get; }

		public abstract bool MutateBoard(ChessPiece?[,] board);
	}

	public class NormalChessMove : ChessMove
	{
		public override string MoveId => $"normal-{From.X}/{From.Y}-{To.X}/{To.Y}";

		public override bool MutateBoard(ChessPiece?[,] board)
		{
			ChessPiece? from = board[From.X, From.Y];
			if (from == null) return false;

			board[From.X, From.Y] = null;
			board[To.X, To.Y] = from;

			return true;
		}
	}

	public class EnPassantChessMove : ChessMove
	{
		public override string MoveId => $"enpassant-{From.X}/{From.Y}-{To.X}/{To.Y}";

		public override bool MutateBoard(ChessPiece?[,] board)
		{
			ChessPiece? from = board[From.X, From.Y];
			if (from == null) return false;

			board[From.X, From.Y] = null;
			board[To.X, To.Y] = from;

			// Captured pawn sits in the destination file on the mover's rank
			board[To.X, From.Y] = null;

			return true;
		}
	}

	public class CastlingChessMove : ChessMove
	{
		public override string MoveId => $"castle-{From.X}/{From.Y}-{To.X}/{To.Y}";

		public override bool MutateBoard(ChessPiece?[,] board)
		{
			ChessPiece? king = board[From.X, From.Y];
			if (king == null) return false;

			int direction = Math.Sign(To.X - From.X);
			if (direction == 0) return false;

			// Rook sits at the board edge in the castling direction
			int rookX = direction > 0 ? board.GetLength(0) - 1 : 0;
			ChessPiece? rook = board[rookX, From.Y];
			if (rook == null) return false;

			board[From.X, From.Y] = null;
			board[To.X, To.Y] = king;

			// Rook jumps to the square the king passed over
			board[rookX, From.Y] = null;
			board[From.X + direction, From.Y] = rook;

			return true;
		}
	}

	public class PromotionChessMove : ChessMove
	{
		public required ChessPiece PromotionPiece { get; set; }

		public override string MoveId => $"promotion-{From.X}/{From.Y}-{To.X}/{To.Y}-{PromotionPiece.Type}";


		public override bool MutateBoard(ChessPiece?[,] board)
		{
			ChessPiece? from = board[From.X, From.Y];
			if (from == null) return false;

			board[From.X, From.Y] = null;
			board[To.X, To.Y] = new ChessPiece(PromotionPiece.Type, from.PlayerId, from.Color);

			return true;
		}
	}
}
