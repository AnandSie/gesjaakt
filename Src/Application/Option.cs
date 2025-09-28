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

public interface IGameOption : IOption { }

// TODO: double extensions feels weird- I need the seperate IgameOption (withoutT)
public class GameOption<T> : Option, IGameOption where T : IGame
{
    public GameOption() : base(typeof(T).Name, typeof(T))
    { }
}