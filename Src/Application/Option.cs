using Domain.Interfaces.Games.BaseGame;

namespace Application;

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

public interface IGameOption : IOption
{
    public int MinNumberOfPlayers { get; }
    public int MaxNumberOfPlayers { get; }

}

public class GameOption<T> : Option, IGameOption where T : IGame
{
    public int MinNumberOfPlayers { get; }
    public int MaxNumberOfPlayers { get; }

    public GameOption(int minNumberOfPlayers, int maxNumberOfPlayers) : base(typeof(T).Name, typeof(T))
    {
        MinNumberOfPlayers = minNumberOfPlayers;
        MaxNumberOfPlayers = maxNumberOfPlayers;
    }
}