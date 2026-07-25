namespace Domain.Entities.Game.Qwixx;

// See docs/qwixx/rules.md for the rule IDs (QX-###) referenced from QwixxRowTests.
public class QwixxRow
{
    private readonly bool _ascending;
    private readonly HashSet<int> _markedNumbers = new();
    private int? _lastMarkedNumber;
    private int _numbersMarkedCount;
    private bool _locked;

    public QwixxRow(QwixxColor color)
    {
        Color = color;
        _ascending = color is QwixxColor.Red or QwixxColor.Yellow;
    }

    public QwixxColor Color { get; }

    public bool IsLocked => _locked;

    // The lock cell counts as a mark of its own (QX-005/QX-028), on top of the numbers marked -
    // so a fully marked and locked row is 11 numbers + 1 lock cell = 12.
    public int MarkedCount => _numbersMarkedCount + (_locked ? 1 : 0);

    public int Score => QwixxRules.RowScoreByMarkedCount[MarkedCount];

    // QX-015/QX-016/QX-017/QX-021: whether `number` can still be marked given what's already marked.
    public bool CanMark(int number)
    {
        // QX-003/QX-004: every row lists 2..12 and nothing else, so anything outside that was
        // never printed on the sheet and can't be crossed out however the row currently stands.
        if (number < QwixxRules.MinRowNumber || number > QwixxRules.MaxRowNumber)
        {
            return false;
        }

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
        _markedNumbers.Add(number);
    }

    // Distinguishes an actually-marked number from one that was skipped over (QX-017): both
    // become permanently unmarkable, but only a marked one was really crossed out.
    public bool IsMarked(int number) => _markedNumbers.Contains(number);

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
