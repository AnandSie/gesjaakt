using Domain.Entities.Events;

namespace Application.Interfaces;

public interface IGameRunner
{
    public void ManualGame(int numberOfPlayers);
    public void Simulate(int numberOfSimulations);
    public void SimulateAllPossiblePlayerCombis();
    public void ShowStatistics();

    // Events
    public event EventHandler<WarningEvent>? GameEnded;
    public event EventHandler<ErrorEvent>? SimIterStarting;
    public event EventHandler<WarningEvent>? SimIterEnded;
    public event EventHandler<ErrorEvent>? AllSimItersEnded;
    public event EventHandler<CriticalEvent>? PlayerCombinationIterStarting;
    public event EventHandler<CriticalEvent>? PlayerCombinationIterEnded;
}
