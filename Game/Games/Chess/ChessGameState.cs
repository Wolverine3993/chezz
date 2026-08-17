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
	}

	public enum ChessGameResult
	{
		NoResult,
		Win,
		Loss,
		Draw
	}
}
