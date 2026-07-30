using Chezz.Game.Func;
using System.Text.Json.Serialization;

namespace Chezz.Game.Games.Chess;

public class ChessPiece : IPiece
{
	[JsonConverter(typeof(JsonStringEnumConverter))]

	public enum ChessPieceEnum
	{
		Pawn,
		Knight,
		Bishop,
		Rook,
		Queen,
		King,
	}

	[JsonConverter(typeof(JsonStringEnumConverter))]
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

	public string ImageUrl
	{
		get
		{
			return $"/assets/chess/{Color.ToString()}/{Type.ToString()}.svg".ToLower();
		}
	}
}