using Chezz.Game.Func;

namespace Chezz.Game.Games.Chess;

public class ChessPiece : IPiece
{
	public enum ChessPieceEnum
	{
		Pawn,
		Knight,
		Bishop,
		Rook,
		Queen,
		King,
	}
	public enum PieceColor
	{
		White,
		Black
	}

	public ChessPieceEnum Type { get; }
	public string PlayerId
	{
		get;
	}
	public PieceColor Color { get; }

	public ChessPiece(ChessPieceEnum type, string playerId, PieceColor color)
	{
		this.Type = type;
		this.PlayerId = playerId;
		this.Color = color;
	}

	public string ImageUrl()
	{
		return $"/assets/chess/{Color.ToString()}/{Type.ToString().ToLower()}.svg";
	}
}