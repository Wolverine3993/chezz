using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game.Games.Chess;

public class ChessGameImplementation : IGameImplementation<ChessPiece, ChessMove, ChessGameState, ChessGameStore>
{
	public int CurrentTurn { get; set; }

	private static readonly (int dx, int dy)[] RookDirections =
	{
		(1, 0), (-1, 0), (0, 1), (0, -1)
	};

	private static readonly (int dx, int dy)[] BishopDirections =
	{
		(1, 1), (1, -1), (-1, 1), (-1, -1)
	};

	private static readonly (int dx, int dy)[] KnightOffsets =
	{
		(1, 2), (2, 1), (2, -1), (1, -2), (-1, -2), (-2, -1), (-2, 1), (-1, 2)
	};

	private static readonly (int dx, int dy)[] KingOffsets =
	{
		(1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1)
	};

	public List<ChessMove> GetValidMoves(ChessGameStore gameStore, IPlayer player)
	{
		List<ChessMove> moves = new();

		ChessPiece?[,] board = gameStore.Board;
		int width = board.GetLength(0);
		int height = board.GetLength(1);

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				ChessPiece? piece = board[x, y];
				if (piece == null) continue;

				if (piece.PlayerId != player.Id) continue;

				switch (piece.Type)
				{
					case ChessPiece.ChessPieceEnum.Pawn:
						AddPawnMoves(gameStore, board, moves, x, y, piece, width, height);
						break;
					case ChessPiece.ChessPieceEnum.Knight:
						AddStepMoves(board, moves, x, y, piece, KnightOffsets, width, height);
						break;
					case ChessPiece.ChessPieceEnum.Bishop:
						AddSlidingMoves(board, moves, x, y, piece, BishopDirections, width, height);
						break;
					case ChessPiece.ChessPieceEnum.Rook:
						AddSlidingMoves(board, moves, x, y, piece, RookDirections, width, height);
						break;
					case ChessPiece.ChessPieceEnum.Queen:
						AddSlidingMoves(board, moves, x, y, piece, RookDirections, width, height);
						AddSlidingMoves(board, moves, x, y, piece, BishopDirections, width, height);
						break;
					case ChessPiece.ChessPieceEnum.King:
						AddStepMoves(board, moves, x, y, piece, KingOffsets, width, height);
						AddCastlingMoves(gameStore, board, moves, x, y, piece, width, height);
						break;
				}
			}
		}

		// Drop moves that leave the mover's own king in check
		moves.RemoveAll(move => WouldLeaveKingInCheck(board, move, player.Id, width, height));

		return moves;
	}

	private static bool InBounds(int x, int y, int width, int height)
		=> x >= 0 && x < width && y >= 0 && y < height;

	private static ChessPiece?[,] CloneBoard(ChessPiece?[,] board)
	{
		ChessPiece?[,] copy = new ChessPiece?[board.GetLength(0), board.GetLength(1)];
		Array.Copy(board, copy, board.Length);
		return copy;
	}

	private static bool WouldLeaveKingInCheck(ChessPiece?[,] board, ChessMove move, string playerId,
		int width, int height)
	{
		ChessPiece?[,] copy = CloneBoard(board);
		move.MutateBoard(copy);

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				ChessPiece? piece = copy[x, y];
				if (piece != null && piece.PlayerId == playerId && piece.Type == ChessPiece.ChessPieceEnum.King)
				{
					return IsSquareAttacked(copy, x, y, piece.Color, playerId, width, height);
				}
			}
		}

		// No king on the board
		return false;
	}

	private static bool IsSquareAttacked(ChessPiece?[,] board, int tx, int ty,
		ChessPiece.PieceColor defenderColor, string defenderPlayerId, int width, int height)
	{
		// Attacking pawn sits one rank in front of the square
		int opponentDirection = defenderColor == ChessPiece.PieceColor.White ? 1 : -1;
		int pawnRow = ty - opponentDirection;
		foreach (int px in stackalloc[] { tx - 1, tx + 1 })
		{
			if (!InBounds(px, pawnRow, width, height)) continue;
			ChessPiece? p = board[px, pawnRow];
			if (IsEnemy(p, defenderPlayerId) && p!.Type == ChessPiece.ChessPieceEnum.Pawn) return true;
		}

		// Knight attacks
		foreach ((int dx, int dy) in KnightOffsets)
		{
			int nx = tx + dx;
			int ny = ty + dy;
			if (!InBounds(nx, ny, width, height)) continue;
			ChessPiece? p = board[nx, ny];
			if (IsEnemy(p, defenderPlayerId) && p!.Type == ChessPiece.ChessPieceEnum.Knight) return true;
		}

		// Adjacent enemy king
		foreach ((int dx, int dy) in KingOffsets)
		{
			int nx = tx + dx;
			int ny = ty + dy;
			if (!InBounds(nx, ny, width, height)) continue;
			ChessPiece? p = board[nx, ny];
			if (IsEnemy(p, defenderPlayerId) && p!.Type == ChessPiece.ChessPieceEnum.King) return true;
		}

		// Orthogonal sliders (rook or queen)
		if (IsAttackedBySlider(board, tx, ty, defenderPlayerId, RookDirections,
			ChessPiece.ChessPieceEnum.Rook, width, height)) return true;

		// Diagonal sliders (bishop or queen)
		if (IsAttackedBySlider(board, tx, ty, defenderPlayerId, BishopDirections,
			ChessPiece.ChessPieceEnum.Bishop, width, height)) return true;

		return false;
	}

	private static bool IsAttackedBySlider(ChessPiece?[,] board, int tx, int ty, string defenderPlayerId,
		(int dx, int dy)[] directions, ChessPiece.ChessPieceEnum sliderType, int width, int height)
	{
		foreach ((int dx, int dy) in directions)
		{
			int nx = tx + dx;
			int ny = ty + dy;
			while (InBounds(nx, ny, width, height))
			{
				ChessPiece? p = board[nx, ny];
				if (p != null)
				{
					if (IsEnemy(p, defenderPlayerId)
						&& (p.Type == sliderType || p.Type == ChessPiece.ChessPieceEnum.Queen))
					{
						return true;
					}
					break;
				}

				nx += dx;
				ny += dy;
			}
		}

		return false;
	}

	private static bool IsEnemy(ChessPiece? piece, string defenderPlayerId)
		=> piece != null && piece.PlayerId != defenderPlayerId;

	private static ChessMove CreateMove(int fromX, int fromY, int toX, int toY)
		=> new NormalChessMove { From = (fromX, fromY), To = (toX, toY) };

	private static ChessMove CreateEnPassantMove(int fromX, int fromY, int toX, int toY)
		=> new EnPassantChessMove { From = (fromX, fromY), To = (toX, toY) };

	private static ChessMove CreateCastlingMove(int fromX, int fromY, int toX, int toY)
		=> new CastlingChessMove { From = (fromX, fromY), To = (toX, toY) };

	private static ChessMove CreatePromotionMove(int fromX, int fromY, int toX, int toY,
		ChessPiece.ChessPieceEnum promotionPiece, ChessPiece piece)
		=> new PromotionChessMove
		{
			From = (fromX, fromY),
			To = (toX, toY),
			PromotionPiece = new ChessPiece(promotionPiece, piece.PlayerId, piece.Color)
		};

	private static void AddCastlingMoves(ChessGameStore gameStore, ChessPiece?[,] board, List<ChessMove> moves,
		int x, int y, ChessPiece king, int width, int height)
	{
		// King must not have moved and must not be in check
		if (gameStore.HasMovedFrom((x, y))) return;
		if (IsSquareAttacked(board, x, y, king.Color, king.PlayerId, width, height)) return;

		foreach (int direction in stackalloc[] { -1, 1 })
		{
			int rookX = direction > 0 ? width - 1 : 0;
			ChessPiece? rook = board[rookX, y];
			if (rook == null || rook.Type != ChessPiece.ChessPieceEnum.Rook || rook.PlayerId != king.PlayerId) continue;
			if (gameStore.HasMovedFrom((rookX, y))) continue;

			// Squares between king and rook must be empty
			bool pathClear = true;
			for (int ix = Math.Min(x, rookX) + 1; ix < Math.Max(x, rookX); ix++)
			{
				if (board[ix, y] != null) { pathClear = false; break; }
			}
			if (!pathClear) continue;

			int step1 = x + direction;
			int step2 = x + (2 * direction);
			if (!InBounds(step2, y, width, height)) continue;

			// King may not pass through or land on an attacked square
			if (IsSquareAttacked(board, step1, y, king.Color, king.PlayerId, width, height)) continue;
			if (IsSquareAttacked(board, step2, y, king.Color, king.PlayerId, width, height)) continue;

			moves.Add(CreateCastlingMove(x, y, step2, y));
		}
	}

	private static void AddSlidingMoves(ChessPiece?[,] board, List<ChessMove> moves, int x, int y,
		ChessPiece piece, (int dx, int dy)[] directions, int width, int height)
	{
		foreach ((int dx, int dy) in directions)
		{
			int nx = x + dx;
			int ny = y + dy;
			while (InBounds(nx, ny, width, height))
			{
				ChessPiece? target = board[nx, ny];
				if (target == null)
				{
					moves.Add(CreateMove(x, y, nx, ny));
				}
				else
				{
					if (target.PlayerId != piece.PlayerId) moves.Add(CreateMove(x, y, nx, ny));
					break;
				}

				nx += dx;
				ny += dy;
			}
		}
	}

	private static void AddStepMoves(ChessPiece?[,] board, List<ChessMove> moves, int x, int y,
		ChessPiece piece, (int dx, int dy)[] offsets, int width, int height)
	{
		foreach ((int dx, int dy) in offsets)
		{
			int nx = x + dx;
			int ny = y + dy;
			if (!InBounds(nx, ny, width, height)) continue;

			ChessPiece? target = board[nx, ny];
			if (target == null || target.PlayerId != piece.PlayerId)
			{
				moves.Add(CreateMove(x, y, nx, ny));
			}
		}
	}

	private static void AddPawnMoves(ChessGameStore gameStore, ChessPiece?[,] board, List<ChessMove> moves,
		int x, int y, ChessPiece piece, int width, int height)
	{
		// White is at the bottom (high y) and advances toward y = 0
		int direction = piece.Color == ChessPiece.PieceColor.White ? -1 : 1;
		int startRow = piece.Color == ChessPiece.PieceColor.White ? height - 2 : 1;
		int promotionRow = piece.Color == ChessPiece.PieceColor.White ? 0 : height - 1;

		int oneY = y + direction;
		if (InBounds(x, oneY, width, height) && board[x, oneY] == null)
		{
			AddPawnAdvanceOrPromotion(moves, x, y, x, oneY, promotionRow, piece);

			int twoY = y + (2 * direction);
			if (y == startRow && InBounds(x, twoY, width, height) && board[x, twoY] == null)
			{
				moves.Add(CreateMove(x, y, x, twoY));
			}
		}

		foreach (int dx in stackalloc[] { -1, 1 })
		{
			int nx = x + dx;
			int ny = y + direction;
			if (!InBounds(nx, ny, width, height)) continue;

			ChessPiece? target = board[nx, ny];
			if (target != null)
			{
				if (target.PlayerId != piece.PlayerId) AddPawnAdvanceOrPromotion(moves, x, y, nx, ny, promotionRow, piece);
				continue;
			}

			if (IsEnPassant(gameStore, board, x, y, nx, direction, piece))
			{
				moves.Add(CreateEnPassantMove(x, y, nx, ny));
			}
		}
	}

	private static readonly ChessPiece.ChessPieceEnum[] PromotionPieces =
	{
		ChessPiece.ChessPieceEnum.Queen,
		ChessPiece.ChessPieceEnum.Rook,
		ChessPiece.ChessPieceEnum.Bishop,
		ChessPiece.ChessPieceEnum.Knight,
	};

	private static void AddPawnAdvanceOrPromotion(List<ChessMove> moves, int fromX, int fromY,
		int toX, int toY, int promotionRow, ChessPiece piece)
	{
		if (toY == promotionRow)
		{
			foreach (ChessPiece.ChessPieceEnum promotionPiece in PromotionPieces)
			{
				moves.Add(CreatePromotionMove(fromX, fromY, toX, toY, promotionPiece, piece));
			}
		}
		else
		{
			moves.Add(CreateMove(fromX, fromY, toX, toY));
		}
	}

	private static bool IsEnPassant(ChessGameStore gameStore, ChessPiece?[,] board, int x, int y,
		int captureX, int direction, ChessPiece piece)
	{
		ChessMove? last = gameStore.LastMove;
		if (last == null) return false;

		// Pawn to capture sits beside us on our own rank
		ChessPiece? victim = board[captureX, y];
		if (victim == null) return false;
		if (victim.Type != ChessPiece.ChessPieceEnum.Pawn) return false;
		if (victim.PlayerId == piece.PlayerId) return false;

		// Victim must have advanced two squares last turn
		return last.To.X == captureX && last.To.Y == y
			&& last.From.X == captureX && last.From.Y == y + (2 * direction);
	}

	public void OnMakeMove(ChessGameStore gameStore, IPlayer player, ChessMove move)
	{
		CurrentTurn += 1;
		CurrentTurn %= gameStore.Lobby.Players.Count;
	}
	public ChessGameState GetSerializedState(ChessGameStore gameStore, IPlayer player)
	{
		int playerIndex = gameStore.Lobby.Players.FindIndex(v => v.Id == player.Id);

		return new()
		{
			PackedBoard = gameStore.ToPackedBoard(),
			YourTurn = CurrentTurn == playerIndex,
			YourColor = playerIndex == 0 ? ChessPiece.PieceColor.White : ChessPiece.PieceColor.Black,
			GameResult = DetermineResult(gameStore, player),
		};
	}

	private ChessGameResult DetermineResult(ChessGameStore gameStore, IPlayer player)
	{
		IReadOnlyList<IPlayer> players = gameStore.Lobby.Players;
		if (CurrentTurn < 0 || CurrentTurn >= players.Count) return ChessGameResult.NoResult;

		IPlayer playerToMove = players[CurrentTurn];

		// While the player to move still has a legal move, the game is ongoing.
		if (GetValidMoves(gameStore, playerToMove).Count > 0) return ChessGameResult.NoResult;

		// No legal moves: checkmate if in check, otherwise stalemate (draw).
		if (!IsPlayerInCheck(gameStore, playerToMove)) return ChessGameResult.Draw;

		// Checkmate: the player to move loses, everyone else wins.
		return player.Id == playerToMove.Id ? ChessGameResult.Loss : ChessGameResult.Win;
	}

	private static bool IsPlayerInCheck(ChessGameStore gameStore, IPlayer player)
	{
		ChessPiece?[,] board = gameStore.Board;
		int width = board.GetLength(0);
		int height = board.GetLength(1);

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				ChessPiece? piece = board[x, y];
				if (piece != null && piece.PlayerId == player.Id && piece.Type == ChessPiece.ChessPieceEnum.King)
				{
					return IsSquareAttacked(board, x, y, piece.Color, player.Id, width, height);
				}
			}
		}

		return false;
	}

}