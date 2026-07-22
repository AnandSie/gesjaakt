namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxRowTests.
public class QwixxRow
{
    public QwixxRow(QwixxColor color)
    {
        throw new NotImplementedException();
    }

    public QwixxColor Color => throw new NotImplementedException();

    // Derived from MarkedCount, not separately tracked state: 12 total marks (11 numbers + the
    // lock cell) is only reachable by having also marked the lock, so IsLocked == (MarkedCount == 12).
    public bool IsLocked => throw new NotImplementedException();

    public int MarkedCount => throw new NotImplementedException();

    public int Score => throw new NotImplementedException();

    // QX-015/QX-016/QX-017/QX-021: whether `number` can still be marked given what's already marked.
    public bool CanMark(int number)
    {
        throw new NotImplementedException();
    }

    public void Mark(int number)
    {
        throw new NotImplementedException();
    }

    // QX-022: only true once the row's last number is marked and total marks (including it) is >= 5.
    public bool CanLock()
    {
        throw new NotImplementedException();
    }

    public void Lock()
    {
        throw new NotImplementedException();
    }
}
