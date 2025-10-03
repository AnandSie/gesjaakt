using Domain.Interfaces.Games.BaseGame;

namespace Application;

// REFACTOR - seperate files
public interface IOption
{
    string Name { get; }
    Type Type { get; }
}

public class Option : IOption
{
    public string Name { get; }
    public Type Type { get; }

    public Option(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}


public abstract class GameOption : Option, IRangeOfPlayers
{
    public int MinNumberOfPlayers { get; }
    public int MaxNumberOfPlayers { get; }

    public GameOption(Type game, int minNumberOfPlayers, int maxNumberOfPlayers) : base(game.Name, game)
    {
        MinNumberOfPlayers = minNumberOfPlayers;
        MaxNumberOfPlayers = maxNumberOfPlayers;
    }
}