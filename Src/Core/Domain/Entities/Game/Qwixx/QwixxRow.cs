namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxRowTests.
public class QwixxRow
{
    private readonly bool _ascending;
    private int? _lastMarkedNumber;
    private int _numbersMarkedCount;
    private bool _locked;

    public QwixxRow(QwixxColor color)
    {
        Color = color;
        _ascending = color is QwixxColor.Red or QwixxColor.Yellow;
    }

    public QwixxColor Color { get; }

    // Derived from MarkedCount, not separately tracked state: 12 total marks (11 numbers + the
    // lock cell) is only reachable by having also marked the lock, so IsLocked == (MarkedCount == 12).
    public bool IsLocked => _locked;

    public int MarkedCount => _numbersMarkedCount + (_locked ? 1 : 0);

    public int Score => QwixxRules.RowScoreByMarkedCount[MarkedCount];

    // QX-015/QX-016/QX-017/QX-021: whether `number` can still be marked given what's already marked.
    public bool CanMark(int number)
    {
        if (_locked)
        {
            return false;
        }

        if (_lastMarkedNumber is null)
        {
            return true;
        }

        return _ascending ? number > _lastMarkedNumber : number < _lastMarkedNumber;
    }

    public void Mark(int number)
    {
        if (!CanMark(number))
        {
            throw new InvalidOperationException($"Cannot mark {number} on this row.");
        }

        _lastMarkedNumber = number;
        _numbersMarkedCount++;
    }

    // QX-022: only true once the row's last number is marked and total marks (including it) is >= 5.
    public bool CanLock()
    {
        if (_locked)
        {
            return false;
        }

        var lastRowNumber = _ascending ? QwixxRules.MaxRowNumber : QwixxRules.MinRowNumber;
        return _lastMarkedNumber == lastRowNumber && _numbersMarkedCount >= QwixxRules.MinMarksToLock;
    }

    public void Lock()
    {
        if (!CanLock())
        {
            throw new InvalidOperationException("Cannot lock this row.");
        }

        _locked = true;
    }
}
