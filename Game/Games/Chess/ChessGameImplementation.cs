using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game.Games.Chess;

public class ChessGameImplementation : IGameImplementation<ChessPiece, ChessMove, ChessGameState, ChessGameStore>
{
	public int CurrentTurn { get; set; }

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
		CurrentTurn %= gameStore.Lobby.Players.Count;
	}
	public ChessGameState GetStatus(ChessGameStore gameStore, IPlayer player)
	{
		int width = gameStore.Board.GetLength(0);
		int height = gameStore.Board.GetLength(1);
		ChessPiece?[][] board = new ChessPiece?[width][];
		for (int x = 0; x < width; x++)
		{
			board[x] = new ChessPiece?[height];
			for (int y = 0; y < height; y++)
			{
				board[x][y] = gameStore.Board[x, y];
			}
		}

		return new()
		{
			Board = board,
			YourTurn = CurrentTurn == gameStore.Lobby.Players.FindIndex(v => v.Id == player.Id),
		};
	}

}