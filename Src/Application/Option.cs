namespace Application;

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
