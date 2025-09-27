using Domain.Interfaces.Games.BaseGame;

namespace Application;

public class Option
{
    public string Name { get; }
    public Type Type { get; }

    public Option(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}


public class GameOption<T> : Option where T : IGame
{
    public GameOption() : base(typeof(T).Name, typeof(T))
    { }
}