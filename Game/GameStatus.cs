using Chezz.Game.Players;

namespace Chezz.Game
{
	public abstract record GameStatus
	{
		private GameStatus() { }

		public sealed record Playing : GameStatus;

		public sealed record Ended(IReadOnlyList<IPlayer> Winners) : GameStatus
		{
			public bool IsDraw => Winners.Count == 0;

			public static Ended Draw { get; } = new([]);

			public static Ended Winner(IPlayer winner) => new([winner]);
		}
	}
}
