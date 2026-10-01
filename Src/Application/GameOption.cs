using Domain.Interfaces.Games.BaseGame;

namespace Application;

public abstract class GameOption : Option, IAmountOfPlayers
{
    public int MinNumberOfPlayers { get; }
    public int MaxNumberOfPlayers { get; }

    public GameOption(Type game, int minNumberOfPlayers, int maxNumberOfPlayers) : base(game.Name, game)
    {
        MinNumberOfPlayers = minNumberOfPlayers;
        MaxNumberOfPlayers = maxNumberOfPlayers;
    }
}
