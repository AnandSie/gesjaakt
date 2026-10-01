namespace Application;

public class ActionOption(string name, Action method): IOption
{
    public string Name { get; } = name;
    public Action Action { get; } = method;
}
