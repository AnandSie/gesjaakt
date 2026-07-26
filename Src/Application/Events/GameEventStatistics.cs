using Application.Interfaces;
using Domain.Entities.Events;

namespace Application.Events;

public class GameEventStatistics : IGameEventStatistics
{
    // GameRunner raises this once per finished game (it passes nameof(GameEnded)),
    // which is what lets us express everything else as a per-game average without
    // the statistics needing a reference to the runner.
    public const string GameEndedKind = "GameEnded";

    private readonly Dictionary<string, EventStatistic> _byKind = [];
    private int _gamesObserved;

    public void Record(GameEvent gameEvent)
    {
        if (!_byKind.TryGetValue(gameEvent.Kind, out var statistic))
        {
            statistic = new EventStatistic(gameEvent.Kind, gameEvent.Importance, gameEvent.Category);
            _byKind[gameEvent.Kind] = statistic;
        }

        statistic.Add(gameEvent);

        if (gameEvent.Kind == GameEndedKind)
        {
            _gamesObserved++;
        }
    }

    public EventStatisticsReport Report()
    {
        var ordered = _byKind.Values
            .OrderByDescending(s => s.Category == EventCategory.Fault)
            .ThenByDescending(s => s.Count)
            .ToList();

        return new EventStatisticsReport(_gamesObserved, ordered);
    }

    public void Reset()
    {
        _byKind.Clear();
        _gamesObserved = 0;
    }
}
