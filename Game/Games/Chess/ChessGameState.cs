namespace Chezz.Game.Games.Chess
{
	public class ChessGameState
	{
		public required ChessPiece?[][] Board { get; set; }
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
