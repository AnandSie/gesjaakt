namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxDiceRollTests.
public class QwixxDiceRoll
{
    private readonly int _red;
    private readonly int _yellow;
    private readonly int _green;
    private readonly int _blue;

    public QwixxDiceRoll(int white1, int white2, int red, int yellow, int green, int blue)
    {
        White1 = white1;
        White2 = white2;
        _red = red;
        _yellow = yellow;
        _green = green;
        _blue = blue;
    }

    public int White1 { get; }

    public int White2 { get; }

    // QX-009: sum of the two white dice, available to every player.
    public int WhiteSum => White1 + White2;

    // QX-010: the two candidate sums (one per white die) for the given color's die,
    // available only to the active player.
    public IReadOnlyList<int> ColoredSums(QwixxColor color)
    {
        var colorDie = color switch
        {
            QwixxColor.Red => _red,
            QwixxColor.Yellow => _yellow,
            QwixxColor.Green => _green,
            QwixxColor.Blue => _blue,
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };

        return new[] { White1 + colorDie, White2 + colorDie };
    }
}
