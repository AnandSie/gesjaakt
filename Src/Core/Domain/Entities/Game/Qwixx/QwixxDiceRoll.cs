namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxDiceRollTests.
public class QwixxDiceRoll
{
    public QwixxDiceRoll(int white1, int white2, int red, int yellow, int green, int blue)
    {
        throw new NotImplementedException();
    }

    public int White1 => throw new NotImplementedException();

    public int White2 => throw new NotImplementedException();

    // QX-009: sum of the two white dice, available to every player.
    public int WhiteSum => throw new NotImplementedException();

    // QX-010: the two candidate sums (one per white die) for the given color's die,
    // available only to the active player.
    public IReadOnlyList<int> ColoredSums(QwixxColor color)
    {
        throw new NotImplementedException();
    }
}
