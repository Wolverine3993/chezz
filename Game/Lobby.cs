using Chezz.Game.Players;

namespace Chezz.Game;

public class Lobby
{
    private readonly int maxPlayers;
    
    public List<IPlayer> players;

    public Lobby(int maxPlayers)
    {
        this.maxPlayers = maxPlayers;
    }
}