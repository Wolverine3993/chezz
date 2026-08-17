using Chezz.Game.Players;

namespace Chezz.Game.Games.Chess
{
	public class PackedBoardState
	{
		public required string PackedBoard { get; set; }
		public required Dictionary<string, string> ImageUrls { get; set; }
	}
	public class ChessGameState
	{
		public required PackedBoardState PackedBoard { get; set; }
		public required bool YourTurn { get; set; }
		public required ChessPiece.PieceColor YourColor { get; set; }
		public required ChessGameResult GameResult { get; set; }
		public required List<string> MoveHistory { get; set; }
		public required PlayerMetadata? WhitePlayer { get; set; }
		public required PlayerMetadata? BlackPlayer { get; set; }
	}

	public enum ChessGameResult
	{
		NoResult,
		Win,
		Loss,
		Draw
	}
}
