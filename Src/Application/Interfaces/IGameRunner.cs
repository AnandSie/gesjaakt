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
    public event EventHandler<ErrorEvent>? GameSimulationStarting;
    public event EventHandler<ErrorEvent>? GameSimulationEnded;
    public event EventHandler<CriticalEvent>? PlayerCombinationSimulationStarting;
    public event EventHandler<CriticalEvent>? PlayerCombinationSimulationEnded;
}
