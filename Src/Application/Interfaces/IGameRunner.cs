using Domain.Entities.Events;

namespace Application.Interfaces;

public interface IGameRunner
{
    public void ManualGame(int numberOfPlayers);
    public void Simulate(int numberOfSimulations);
    public void SimulateAllPossibleCombis();
    public void ShowStatistics();

    // Events
    public event EventHandler<WarningEvent>? GameEnded;
    public event EventHandler<InfoEvent>? GameSimulationStarted;
    public event EventHandler<InfoEvent>? GameSimulationEnded;
    public event EventHandler<CriticalEvent>? PlayerCombinationSimulationStarted;
    public event EventHandler<CriticalEvent>? PlayerCombinationSimulationEnded;
}
