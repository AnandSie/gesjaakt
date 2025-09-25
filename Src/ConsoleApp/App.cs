using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;

namespace ConsoleApp;

// TODO: don't define the TPlayer in the app...
internal class App
{
    readonly ILogger<App> _logger;
    readonly IPlayerInputProvider _playerInputProvider;
    readonly IGameRunner _gameRunner;

    public App(ILogger<App> logger,
        IPlayerInputProvider playerInputProvider,
        IGameRunner gameRunner)
    {
        _logger = logger;
        _playerInputProvider = playerInputProvider;
        _gameRunner = gameRunner;
    }

    public void Start()
    {
        _logger.LogInformation("Starting application...");


        _logger.LogCritical(
            """
            LETS PLAY!
            What do you want?
            1. Simulated Set Game
            2. Simulated All GamesS
            3. Manual Game
            4. Visualize a thinker
            """
                );

        var choice = _playerInputProvider.GetPlayerInputAsInt(new[] { 1, 2, 3 });
        switch (choice)
        {
            case 1:
                var numberOfSimulations = _playerInputProvider.GetPlayerInputAsIntWithMinMax("How often should we run it", 1, 10000);
                _logger.LogCritical($"Simulation started with {numberOfSimulations} runs");

                _gameRunner.Simulate(numberOfSimulations);
                break;

            case 2:
                _logger.LogCritical($"Simulation started with all possible combination");

                _gameRunner.SimulateAllPossibleCombis();
                break;

            case 3:
                var playersToAdd = _playerInputProvider.GetPlayerInputAsInt("How many players do you want to play (3-7)?", new[] { 3, 4, 5, 6, 7 });
                _gameRunner.ManualGame(playersToAdd);
                break;

            case 4:
                _gameRunner.ShowStatistics();
                break;
        }

        _logger.LogCritical($"Press enter to exit");
        Console.ReadLine();
    }
}
