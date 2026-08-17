using System.Net.WebSockets;
using System.Text;

namespace Chezz.Notifications;

public class NotificationSocket(WebSocket webSocket)
{
	private readonly SemaphoreSlim _sendLock = new(1, 1);

	public Guid Id { get; } = Guid.NewGuid();

	public async Task NotifyAsync()
	{
		if (webSocket.State != WebSocketState.Open) return;

		var message = Encoding.UTF8.GetBytes("notify");
		await _sendLock.WaitAsync();
		try
		{
			await webSocket.SendAsync(message, WebSocketMessageType.Text, true, CancellationToken.None);
		}
		catch
		{
		}
		finally
		{
			_sendLock.Release();
		}
	}

	public async Task ListenUntilClosedAsync()
	{
		var buffer = new byte[1024];
		try
		{
			while (webSocket.State == WebSocketState.Open)
			{
				var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
				if (result.MessageType == WebSocketMessageType.Close) break;
			}
		}
		catch
		{
		}
	}
}
