using Chezz.Game.Func;
using System.Collections.Concurrent;

namespace Chezz.Game
{
	public sealed class GameRegistry<TMove, TPiece, TGameStatus, TGs, TGi>
	where TPiece : IPiece
	where TMove : IMove
	where TGs : IGameState<TMove>
	where TGi : IGameImplementation<TPiece, TMove, TGameStatus, TGs>
	{
		private readonly ConcurrentDictionary<Guid, Game<TMove, TPiece, TGameStatus, TGs, TGi>> games = new();

		public void Add(Game<TMove, TPiece, TGameStatus, TGs, TGi> game) => games[game.Id] = game;
		public Game<TMove, TPiece, TGameStatus, TGs, TGi>? Get(Guid id) => games.TryGetValue(id, out var g) ? g : null;
		public bool Remove(Guid id) => games.TryRemove(id, out _);
		public IReadOnlyCollection<Game<TMove, TPiece, TGameStatus, TGs, TGi>> All => games.Values.ToArray();
	}
}
