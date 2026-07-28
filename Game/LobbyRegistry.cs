using System.Collections.Concurrent;

namespace Chezz.Game
{
	public sealed class LobbyRegistry
	{
		private readonly ConcurrentDictionary<GameType, ConcurrentDictionary<Guid, Lobby>> lobbies = new();

		private ConcurrentDictionary<Guid, Lobby> ForType(GameType gameType)
			=> lobbies.GetOrAdd(gameType, _ => new());

		public void Add(Lobby lobby) => ForType(lobby.GameType)[lobby.Id] = lobby;

		public Lobby? Get(GameType gameType, Guid id)
			=> lobbies.TryGetValue(gameType, out var byId) && byId.TryGetValue(id, out var l) ? l : null;

		public bool Remove(GameType gameType, Guid id)
			=> lobbies.TryGetValue(gameType, out var byId) && byId.TryRemove(id, out _);

		public IReadOnlyCollection<Lobby> All(GameType gameType)
			=> lobbies.TryGetValue(gameType, out var byId) ? byId.Values.ToArray() : [];
	}
}
