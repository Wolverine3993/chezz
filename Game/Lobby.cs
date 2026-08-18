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

	public bool AddPlayer(IPlayer player, ChezzUser? user = null)
	{
		if (_players.Count == maxPlayers) return false;
		_players.Remove(player.UserId);
		_players.Add(player.UserId, player);
		player.SetupDisconnectHandler(() => RemovePlayer(player));
		Notify();
		Console.WriteLine("New player connected, now at " + _players.Count);
		return true;
	}

	public void RemovePlayer(IPlayer player)
	{
		_players.Remove(player.UserId);
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

	public record LobbyInformation(List<string> PlayerUsernames, List<PlayerMetadata> Players, Guid? GameId, bool IsPrivate);

	public LobbyInformation GetLobbyInformation()
	{
		return new LobbyInformation(
			_players.Select(v => v.Value.Username).ToList(),
			_players.Select(v => v.Value.GetMetadata()).ToList(),
			GameId,
			IsPrivate);
	}
}