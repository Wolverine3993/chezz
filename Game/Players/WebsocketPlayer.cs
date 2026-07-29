using Chezz.Database.Models;
using System.Net.WebSockets;
using System.Text;

namespace Chezz.Game.Players
{
	public class WebsocketPlayer : IPlayer
	{
		private WebSocket _webSocket;
		private ChezzUser _user;
		private CancellationToken _cancelToken = new();
		private readonly TaskCompletionSource _closed = new(TaskCreationOptions.RunContinuationsAsynchronously);


		public WebsocketPlayer(WebSocket websocket, ChezzUser _user)
		{
			this._webSocket = websocket;
			this._user = _user;

			var _ = RecieveAsync(_cancelToken);
		}

		public delegate void DelegateOnWebsocketDisconnect();
		public DelegateOnWebsocketDisconnect OnWebsocketDisconnect;


		private string _id = Guid.NewGuid().ToString();
		public string Id => _id;

		public string Username => _user.UserName ?? _user.Email ?? "Unknown user";

		public async Task Notify()
		{
			var message = Encoding.UTF8.GetBytes("notify");
			await _webSocket.SendAsync(message, WebSocketMessageType.Text, true, _cancelToken);
		}

		private async Task RecieveAsync(CancellationToken cancellationToken)
		{
			var buffer = new byte[1024 * 64];

			while (_webSocket.CloseStatus != null)
			{
				var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

				if (result.MessageType == WebSocketMessageType.Close) { break; }

				var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

				Console.WriteLine($"Recieved message from {Id}: {message}");
			}

			if (OnWebsocketDisconnect != null) OnWebsocketDisconnect();
		}

		public void SetupDisconnectHandler(Action onDisconnect)
		{
			OnWebsocketDisconnect += () => onDisconnect();
		}

		public async Task Close()
		{
			try
			{
				if (_webSocket.State == WebSocketState.Open)
					await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
			}
			catch
			{
				// socket may already be gone; ignore
			}
			finally
			{
				_closed.TrySetResult();
			}
		}

		public Task WaitForCloseAsync() => _closed.Task;
	}
}
