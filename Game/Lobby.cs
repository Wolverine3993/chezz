using Chezz.Database.Models;
using Chezz.Game.Players;

namespace Chezz.Game;

public enum GameType
{
	Chess,
}

public class Lobby
{
	private readonly int maxPlayers;

	private Dictionary<string, IPlayer> _players;

	public Guid Id { get; } = Guid.NewGuid();

	public GameType GameType { get; }
	public Guid? GameId { get; set { field = value; Notify(); } } = null;
	public bool IsPrivate { get; private set; }

	public Lobby(int maxPlayers, GameType gameType, bool isPrivate = true)
	{
		this.maxPlayers = maxPlayers;
		IsPrivate = isPrivate;
		GameType = gameType;
		_players = new();
	}

	private void Notify()
	{
		foreach (var player in _players)
		{
			player.Value.Notify();
		}
	}

	public bool AddPlayer(ChezzUser user, IPlayer player)
	{
		if (_players.Count == maxPlayers) return false;
		_players.Remove(user.Id);
		_players.Add(user.Id, player);
		player.SetupDisconnectHandler(() => RemovePlayer(user));
		Notify();
		Console.WriteLine("New player connected, now at " + _players.Count);
		return true;
	}

	public void RemovePlayer(ChezzUser user)
	{
		_players.Remove(user.Id);
		Notify();
	}

	public bool CanStartGame()
	{
		return _players.Count == maxPlayers;
	}

	public void ChangePrivacy(bool isPrivate)
	{
		IsPrivate = isPrivate;
	}

	public IPlayer? FromChezzUser(ChezzUser user)
	{
		return _players.TryGetValue(user.Id, out IPlayer? player) ? player : null;
	}

	public List<IPlayer> Players { get { return _players.Values.ToList(); } }

	public record LobbyInformation(List<string> PlayerUsernames, Guid? GameId, bool IsPrivate);

	public LobbyInformation GetLobbyInformation()
	{
		return new LobbyInformation(_players.Select(v => v.Value.Username).ToList(), GameId, IsPrivate);
	}
}