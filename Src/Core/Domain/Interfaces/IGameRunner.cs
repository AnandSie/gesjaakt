
namespace Domain.Interfaces;

public interface IGameRunner
{
    public void ManualGame(int numberOfPlayers);
    public void Simulate(int numberOfSimulations);
    public void SimulateAllPossibleCombis();
    public void ShowStatistics();
}

