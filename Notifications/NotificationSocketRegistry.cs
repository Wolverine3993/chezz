using System.Collections.Concurrent;

namespace Chezz.Notifications;

public class NotificationSocketRegistry
{
	private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, NotificationSocket>> _connections = new();

	public void Add(string userId, NotificationSocket socket)
	{
		var userSockets = _connections.GetOrAdd(userId, _ => new());
		userSockets[socket.Id] = socket;
	}

	public void Remove(string userId, NotificationSocket socket)
	{
		if (!_connections.TryGetValue(userId, out var userSockets)) return;

		userSockets.TryRemove(socket.Id, out _);
		if (userSockets.IsEmpty) _connections.TryRemove(userId, out _);
	}

	public async Task NotifyAsync(string userId)
	{
		if (!_connections.TryGetValue(userId, out var userSockets)) return;

		await Task.WhenAll(userSockets.Values.Select(socket => socket.NotifyAsync()));
	}
}
