using Chezz.Game.Func;

namespace Chezz.Game.Games.Chess
{
	public readonly record struct ChessPosition(int X, int Y)
	{
		public bool Equals(ChessPosition position)
		{
			return X == position.X &&
				   Y == position.Y;
		}

		public static implicit operator ChessPosition((int, int) tuple) => new(tuple.Item1, tuple.Item2);
		public static implicit operator (int, int)(ChessPosition position) => (position.X, position.Y);
	}

	public class ChessMove : IMove
	{
		public ChessPosition From { get; set; }
		public ChessPosition To { get; set; }

		public override bool Equals(object? obj)
		{
			return obj is ChessMove move &&
				   From.Equals(move.From) &&
				   To.Equals(move.To);
		}
	}
}
