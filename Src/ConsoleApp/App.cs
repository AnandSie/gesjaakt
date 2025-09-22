using Application;
using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;

namespace ConsoleApp;

internal class App
{
    readonly ILogger<App> _logger;
    readonly IPlayerInputProvider _playerInputProvider;
    readonly Func<int, IGame> _gameFactory;

    public App(ILogger<App> logger,
        IPlayerInputProvider playerInputProvider,
        // TODO: nu defined in program.cs, misschien losse factory van maken
        Func<int, IGame> gameFactory)
    {
        _logger = logger;
        _playerInputProvider = playerInputProvider;
        _gameFactory = gameFactory;
    }

    public void Start()
    {
        _logger.LogCritical(
            """
            Which Game?
            1. Gesjaakt
            2. TakeFive
            """
                );
        var gameChoice = _playerInputProvider.GetPlayerInputAsInt(new[] { 1, 2, 3 });
        var game = _gameFactory.Invoke(gameChoice);

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
                game.Simulate();
                Console.ReadLine();
                break;

            case 2:
                game.SimulateAllPossiblePlayerCombinations();
                Console.ReadLine();
                break;

            case 3:
                var playersToAdd = _playerInputProvider.GetPlayerInputAsInt("How many players do you want to play (3-7)?", new[] { 3, 4, 5, 6, 7 });

                game.RunManualGame(playersToAdd);
                _logger.LogCritical($"Press enter to exit");
                Console.ReadLine();

                break;

            case 4:
                game.ShowStatistics();
                break;
        }
    }
}
