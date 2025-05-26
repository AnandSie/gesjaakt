using Application;
using Domain.Entities.Players;
using Domain.Interfaces;
using System.Text;
using Visualization;

namespace ConsoleApp;

internal class App
{
    readonly ILogger<App> _logger;
    readonly IPlayerFactory _playerFactory;
    readonly IPlayerInputProvider _playerInputProvider;
    readonly IGameDealerFactory _dealerFactory;
    readonly ISimulator _simulator;

    public App(ILogger<App> logger, 
        IPlayerFactory playerFactory, 
        IPlayerInputProvider playerInputProvider, 
        IGameDealerFactory dealerFactory, 
        ISimulator simulator)
    {
        _logger = logger;
        _playerFactory = playerFactory;
        _playerInputProvider = playerInputProvider;
        _dealerFactory = dealerFactory;
        _simulator = simulator;
    }

    public void Run()
    {
        _logger.LogCritical(
            """
            LETS PLAY!
            What do you want?
            1. Simulated Set Game
            2. Simulated All Games
            3. Manual Game
            4. Visualize a thinker
            """
                );

        var choice = _playerInputProvider.GetPlayerInputAsInt(new[] { 1, 2, 3 });

        // TODO: Extract class
        switch (choice)
        {
            case 1:
                var numberOfSimulations = _playerInputProvider.GetPlayerInputAsIntWithMinMax("How often should we run it", 1, 10000);
                _logger.LogCritical($"Simulation started with {numberOfSimulations} runs");

                _simulator.Start(numberOfSimulations);

                _logger.LogCritical($"Press enter to exit");
                Console.ReadLine();
                break;

            case 2:
                _logger.LogCritical($"Simulation started with all possible combination");

                _simulator.StartAllCombis();

                _logger.LogCritical($"Press enter to exit");
                Console.ReadLine();
                break;

            case 3:
                var playersToAdd = _playerInputProvider.GetPlayerInputAsInt("How many players do you want to play (3-7)?", new[] { 3, 4, 5, 6, 7 });

                var players = new List<Player>();
                foreach (var i in Enumerable.Range(3, playersToAdd))
                {
                    var name = _playerInputProvider.GetPlayerInput($"Player number {i - 2} what is your name?");
                    players.Add((Player)_playerFactory.CreateHomoSapiens(name, _playerInputProvider));
                }

                var dealerManualGame = _dealerFactory.Create(players);
                dealerManualGame.Play();
                var winnerManualGame = dealerManualGame.Winner();

                var logMessage = new StringBuilder();

                logMessage.AppendLine("-----------------------------");
                logMessage.AppendLine($"\tThe winner is: {winnerManualGame.Name}");

                foreach (var player in players.OrderBy(p => p.CardPoints() - p.CoinsAmount))
                {
                    logMessage.AppendLine($"\t- {player}");
                }
                _logger.LogCritical(logMessage.ToString());

                _logger.LogCritical($"Press enter to exit");
                Console.ReadLine();
                break;

            case 4:
                new Visualizer().Show();
                break;
        }
    }
}
