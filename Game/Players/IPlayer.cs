namespace Chezz.Game.Players;

public interface IPlayer
{
	public string Id { get; }

	public string Username { get; }

	public Task Notify();

	public Task Close();

	public void SetupDisconnectHandler(Action onDisconnect);
}