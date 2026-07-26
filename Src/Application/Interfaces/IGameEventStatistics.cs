using Application.Events;
using Domain.Entities.Events;

namespace Application.Interfaces;

// Accumulates every event a run raises - including the ones too ordinary to be
// displayed - so a simulation can report totals and means afterwards.
public interface IGameEventStatistics
{
    void Record(GameEvent gameEvent);
    EventStatisticsReport Report();
    void Reset();
}
