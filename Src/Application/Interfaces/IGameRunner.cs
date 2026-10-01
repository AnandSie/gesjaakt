using Domain.Entities.Events;

namespace Application.Interfaces;

public interface IGameRunner
{
    public void StartManualGame(int numberOfPlayers);
    public void StartSingleSimulation(int numberOfSimulations);
    public void StartAllPossiblePlayerCombinationSimulation();
    public void ShowStatistics();

    // Events
    public event EventHandler<NotableEvent>? GameEnded;
    public event EventHandler<SpecialEvent>? SimIterStarting;
    public event EventHandler<NotableEvent>? SimIterEnded;
    public event EventHandler<GameChangingEvent>? AllSimItersEnded;
    public event EventHandler<GameChangingEvent>? PlayerCombinationIterStarting;
    public event EventHandler<GameChangingEvent>? PlayerCombinationIterEnded;
}
