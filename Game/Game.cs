using Chezz.Game.Board;
using Chezz.Game.Func;
using Chezz.Game.Players;

namespace Chezz.Game;

public class Game<TPiece, TGs, TGi> : IBoardState, IBoardStyle where TPiece : IPiece
    where TGs : IGameStore<TPiece>
    where TGi : IGameImplementation<TPiece, TGs>
{
    private readonly Dictionary<IPlayer, PlayerStateBase> _players;
    private TGs _gameStore;


    public int CurrentTurn
    {
        get;
        set => field = value % _players.Count;
    } = 0;

    public Game(Lobby lobby, TGs gameStore)
    {
        _players = new();
        foreach (var player in lobby.players)
        {
            _players.Add(player, new PlayerStateBase());
        }

        _gameStore = gameStore;
    }
}