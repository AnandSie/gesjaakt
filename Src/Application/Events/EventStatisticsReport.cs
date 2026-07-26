using Domain.Entities.Events;

namespace Application.Events;

// An immutable snapshot of what happened during a run, ordered so the loudest
// events come first.
public class EventStatisticsReport(int gamesObserved, IReadOnlyList<EventStatistic> statistics)
{
    public int GamesObserved { get; } = gamesObserved;

    // Every kind that was recorded, including the runner narrating its own
    // progress.
    public IReadOnlyList<EventStatistic> Statistics { get; } = statistics;

    // What happened inside the games. This is what a summary is actually about:
    // "the simulation started 1000 times" is bookkeeping, not a statistic, and
    // averaging a progress percentage produces a number that means nothing.
    public IReadOnlyList<EventStatistic> GameStatistics { get; } = statistics
        .Where(s => s.Category is EventCategory.Play or EventCategory.Fault)
        .ToList();

    public long TotalEvents => Statistics.Sum(s => s.Count);
    public long TotalGameEvents => GameStatistics.Sum(s => s.Count);

    public bool IsEmpty => GameStatistics.Count == 0;
}
