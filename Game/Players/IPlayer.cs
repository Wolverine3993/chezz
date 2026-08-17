namespace Chezz.Game.Players;

public record PlayerMetadata(string Name, int Elo, string ImageUrl);

public interface IPlayer
{
	public string Id { get; }

	public string Username { get; }
	public string UserId { get; }

	public PlayerMetadata GetMetadata();

	public Task Notify();

	public Task Close();

	public void SetupDisconnectHandler(Action onDisconnect);
}