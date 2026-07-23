namespace Domain.Entities.Components;

public class Dice
{
    private static readonly Random _random = new();

    private readonly int _min;
    private readonly int _max;

    public Dice(int min, int max)
    {
        _min = min;
        _max = max;
    }

    public int Roll()
    {
        return _random.Next(_min, _max + 1);
    }
}
