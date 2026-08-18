using Chezz.Game.Func;

namespace Chezz.Game.Players;

public class BotPlayer<TMove, TPiece, TGameSerializedState, TGameState, TGameImplementation> : IPlayer
	where TPiece : IPiece
	where TMove : IMove
	where TGameState : IGameState<TMove>
	where TGameImplementation : IGameImplementation<TPiece, TMove, TGameSerializedState, TGameState>
{
	private readonly string _id = Guid.NewGuid().ToString();
	private readonly Lock _moveLock = new();
	private Game<TMove, TPiece, TGameSerializedState, TGameState, TGameImplementation>? _game;
	private bool _isMoving;

	public string Id => _id;
	public string Username => "Random Bot";
	public string UserId => $"bot-{_id}";

	public PlayerMetadata GetMetadata()
	{
		string imageUrl = $"https://api.dicebear.com/9.x/bottts/svg?seed={Uri.EscapeDataString(_id)}";
		return new PlayerMetadata(Username, 800, imageUrl);
	}

	public void AttachGame(Game<TMove, TPiece, TGameSerializedState, TGameState, TGameImplementation> game)
	{
		_game = game;
		_ = TryMoveAsync();
	}

	public Task Notify()
	{
		_ = TryMoveAsync();
		return Task.CompletedTask;
	}

	private async Task TryMoveAsync()
	{
		var game = _game;
		if (game is null || !game.IsTurn(this)) return;

		lock (_moveLock)
		{
			if (_isMoving) return;
			_isMoving = true;
		}

		try
		{
			while (game.IsTurn(this))
			{
				var moves = game.RequestMovesForPlayer(this);
				if (moves.Count == 0) break;

				var move = moves[Random.Shared.Next(moves.Count)];
				if (!await game.MakeMove(this, move.MoveId)) break;
			}
		}
		finally
		{
			lock (_moveLock) _isMoving = false;
		}
	}

	public Task Close() => Task.CompletedTask;

	public void SetupDisconnectHandler(Action onDisconnect) { }
}
