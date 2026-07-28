using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game.Games.Chess;

public class ChessGameImplementation : IGameImplementation<ChessPiece, ChessMove, ChessGameState, ChessGameStore>
{
	public Lobby Lobby { get; set; }

	public int CurrentTurn
	{
		get;
		set => field = value % Lobby.Players.Count;
	} = 0;

	public List<ChessMove> GetValidMoves(ChessGameStore gameStore, IPlayer player)
	{
		List<ChessMove> moves = new();

		for (int x = 0; x < gameStore.Board.GetLength(0); x++)
		{
			for (int y = 0; y < gameStore.Board.GetLength(1); y++)
			{
				ChessPiece? piece = gameStore.Board[x, y];
				if (piece == null) continue;

				if (y + 1 == gameStore.Board.GetLength(1)) continue;
				if (x + 1 == gameStore.Board.GetLength(0)) continue;

				if (piece.PlayerId != player.Id) continue;

				moves.Add(new ChessMove
				{
					from = (x, y),
					to = (x + 1, y + 1)
				});
			}
		}

		return moves;
	}

	public void OnMakeMove(ChessGameStore gameStore, IPlayer player, ChessMove move)
	{
		CurrentTurn += 1;
	}
	public ChessGameState GetStatus(ChessGameStore gameStore, IPlayer player)
	{
		return new()
		{
			Board = gameStore.Board,
			YourTurn = CurrentTurn == Lobby.Players.FindIndex(v => v.Id == player.Id),
		};
	}

}