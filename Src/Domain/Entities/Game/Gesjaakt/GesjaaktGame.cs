using Application;
using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using System.Text;

namespace Domain.Entities.Game.Gesjaakt;

public class GesjaaktGame : IGame
{
    // FIMX:E moet IVIsualizer wel in applicatin
    private readonly IVisualizer _visualizer;
    readonly IGameDealerFactory _dealerFactory;
    readonly IPlayerInputProvider _playerInputProvider;
    readonly IPlayerFactory _playerFactory;
    readonly ILogger<GesjaaktGame> _logger;
    readonly ISimulator _simulator;

    public GesjaaktGame(IVisualizer visualizer,
        ILogger<GesjaaktGame> logger,
        IPlayerFactory playerFactory,
        IPlayerInputProvider playerInputProvider,
        IGameDealerFactory dealerFactory,
        ISimulator simulator)
    {
        _visualizer = visualizer;
        _logger = logger;
        _playerFactory = playerFactory;
        _playerInputProvider = playerInputProvider;
        _dealerFactory = dealerFactory;
        _simulator = simulator;
    }

    public void RunManualGame(int playersToAdd)
    {
        var players = new List<GesjaaktPlayer>();
        foreach (var i in Enumerable.Range(3, playersToAdd))
        {
            var name = _playerInputProvider.GetPlayerInput($"Player number {i - 2} what is your name?");
            players.Add((GesjaaktPlayer)_playerFactory.CreateHomoSapiens(name, _playerInputProvider));
        }

        var dealerManualGame = _dealerFactory.Create(players);
        dealerManualGame.Prepare();
        dealerManualGame.Play();
        var winnerManualGame = dealerManualGame.Winner();

        // TODO: Move to class/infrastructure
        var logMessage = new StringBuilder();
        logMessage.AppendLine("-----------------------------");
        logMessage.AppendLine($"\tThe winner is: {winnerManualGame.Name}");

        foreach (var player in players.OrderBy(p => p.CardPoints() - p.CoinsAmount))
        {
            logMessage.AppendLine($"\t- {player}");
        }
        _logger.LogCritical(logMessage.ToString());
    }

    public void ShowStatistics()
    {
        this._visualizer.Show();
    }

    public void Simulate()
    {
        var numberOfSimulations = _playerInputProvider.GetPlayerInputAsIntWithMinMax("How often should we run it", 1, 10000);
        _logger.LogCritical($"Simulation started with {numberOfSimulations} runs");

        _simulator.Start(numberOfSimulations);

        _logger.LogCritical($"Press enter to exit");
    }

    public void SimulateAllPossiblePlayerCombinations()
    {
        _logger.LogCritical($"Simulation started with all possible combination");
        _simulator.StartAllCombis();
        _logger.LogCritical($"Press enter to exit");
    }
}
