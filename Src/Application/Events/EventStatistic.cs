using Domain.Entities.Events;

namespace Application.Events;

// Everything a simulation learned about one kind of event.
//
// Note this is an aggregate, not a log: a 10.000-game run raises millions of
// events and keeping them all would dwarf the game state itself. Counters and
// running sums give the same summary/avg/mean at constant memory.
public class EventStatistic(string kind, EventImportance importance, EventCategory category)
{
    public string Kind { get; } = kind;
    public EventImportance Importance { get; } = importance;
    public EventCategory Category { get; } = category;

    // How often the event was raised.
    public long Count { get; private set; }

    // How many of those raises carried a numeric Value. Tracked separately from
    // Count because the mean must divide by the values seen, not by the raises.
    public long ValueCount { get; private set; }

    public double ValueSum { get; private set; }
    public double ValueMin { get; private set; } = double.MaxValue;
    public double ValueMax { get; private set; } = double.MinValue;

    // How often this event happened to each player. Empty for events that aren't
    // about anybody in particular, like a card being drawn from the deck.
    private readonly Dictionary<string, long> _countByActor = [];
    public IReadOnlyDictionary<string, long> CountByActor => _countByActor;

    public bool HasActors => _countByActor.Count > 0;
    public long CountFor(string actor) => _countByActor.GetValueOrDefault(actor);

    public long AttributedCount => _countByActor.Values.Sum();

    // Events of a kind that names players, that nonetheless named nobody. Always
    // a bug in the raise site rather than a fact about the game: it means one
    // `?.Invoke` for this event passes `actor:` and another doesn't, which
    // silently understates every player's share.
    public long UnattributedCount => Count - AttributedCount;
    public bool HasUnattributed => HasActors && UnattributedCount > 0;

    public bool HasValues => ValueCount > 0;
    public double? ValueMean => HasValues ? ValueSum / ValueCount : null;
    public double? Minimum => HasValues ? ValueMin : null;
    public double? Maximum => HasValues ? ValueMax : null;

    public void Add(GameEvent gameEvent)
    {
        Count++;

        if (gameEvent.Actor is string actor)
        {
            _countByActor[actor] = _countByActor.GetValueOrDefault(actor) + 1;
        }

        if (gameEvent.Value is not double value) return;

        ValueCount++;
        ValueSum += value;
        ValueMin = Math.Min(ValueMin, value);
        ValueMax = Math.Max(ValueMax, value);
    }

    // Mean number of times this event happened in a single game.
    public double PerGame(int gamesObserved) => gamesObserved == 0 ? Count : (double)Count / gamesObserved;
}
