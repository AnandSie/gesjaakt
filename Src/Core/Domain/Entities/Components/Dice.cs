namespace Domain.Entities.Components;

public class Dice
{
    private readonly int _min;
    private readonly int _max;

    public Dice(int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentOutOfRangeException(nameof(min), min, $"A die's lowest value cannot be above its highest ({max}).");
        }

        _min = min;
        _max = max;
    }

    // Random.Shared rather than a Random of this class's own: instance methods on Random are
    // documented as not thread-safe, and a single Dice can be shared by anything that holds it.
    public int Roll()
    {
        return Random.Shared.Next(_min, _max + 1);
    }
}
