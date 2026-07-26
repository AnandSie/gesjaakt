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

    // The kinds that are about a specific player, and can therefore be broken
    // down per player.
    public IReadOnlyList<EventStatistic> ActorStatistics { get; } = statistics
        .Where(s => s.Category is EventCategory.Play or EventCategory.Fault && s.HasActors)
        .ToList();

    // Every player any event was attributed to, busiest first, so the columns of
    // a per-player breakdown are stable and the most active bot leads.
    public IReadOnlyList<string> Players { get; } = statistics
        .SelectMany(s => s.CountByActor)
        .GroupBy(entry => entry.Key)
        .OrderByDescending(group => group.Sum(entry => entry.Value))
        .Select(group => group.Key)
        .ToList();

    public long TotalEvents => Statistics.Sum(s => s.Count);
    public long TotalGameEvents => GameStatistics.Sum(s => s.Count);

    public bool IsEmpty => GameStatistics.Count == 0;
    public bool HasPlayerBreakdown => ActorStatistics.Count > 0 && Players.Count > 0;

    // Kinds that name a player at some raise sites but not all. The per-player
    // shares divide by every event of the kind, so these rows silently add up to
    // less than 100% - which is why presenters shout about it rather than just
    // rendering a slightly-wrong table.
    public IReadOnlyList<EventStatistic> IncompletelyAttributed { get; } = statistics
        .Where(s => s.Category is EventCategory.Play or EventCategory.Fault && s.HasUnattributed)
        .ToList();

    public bool HasAttributionGap => IncompletelyAttributed.Count > 0;
}
