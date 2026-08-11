namespace Chezz.Game.Players;

public interface IPlayer
{
	public string Id { get; }

	public string Username { get; }

	public Task Notify();

	public Task SendMessage(string message);

	public Task Close();

	public void SetupDisconnectHandler(Action onDisconnect);
}