using Application;
using Application.Interfaces;
using Domain.Entities.Events;

namespace ConsoleApp;

internal class App
{
    private readonly ILogger<App> _logger;
    private readonly List<GameOption> _gameoptions;
    private readonly IPlayerInputProvider _playerInputProvider;
    private readonly GameRunnerFactory _gameRunnerFactory;
    private readonly IGameRunnerEventCollector _gameEventCollector;
    private readonly IGameEventHandler _gameEventHandler;

    public App(ILogger<App> logger,
        List<GameOption> gameoptions,
        IPlayerInputProvider playerInputProvider,
        GameRunnerFactory gameRunnerFactory,
        IGameRunnerEventCollector gameEventCollector,
        IGameEventHandler gameEventHandler)
    {
        _logger = logger;
        _gameoptions = gameoptions;
        _playerInputProvider = playerInputProvider;
        _gameRunnerFactory = gameRunnerFactory;
        _gameEventCollector = gameEventCollector;
        _gameEventHandler = gameEventHandler;
    }

    public void Start()
    {
        var gameOption = GetUsersSelectedGameOption();
        IGameRunner _gameRunner = GetGameRunner(gameOption);
        _gameEventCollector.Attach(_gameRunner);

        // REFACTOR - use class Option to create options
        const string Message = """
            LETS PLAY!
            What do you want?
            1. Simulated Set Game
            2. Simulated All GamesS
            3. Manual Game
            4. Visualize a thinker
            """;

        var choice = _playerInputProvider.GetPlayerInputAsInt(Message, [1, 2, 3, 4]);
        switch (choice)
        {
            case 1:
                _gameEventHandler.SetMinLevel(EventLevel.Error);
                var numberOfSimulations = _playerInputProvider.GetPlayerInputAsIntWithMinMax("How often should we run it", 1, 10000);
                _logger.LogCritical($"Simulation started with {numberOfSimulations} runs");

                _gameRunner.Simulate(numberOfSimulations);
                break;

            case 2:
                _gameEventHandler.SetMinLevel(EventLevel.Critical);

                _logger.LogCritical($"Simulation started with all possible combinations");
                _gameRunner.SimulateAllPossibleCombis();
                break;

            case 3:
                _gameEventHandler.SetMinLevel(EventLevel.Info);

                string question = $"With how many players do you want to play ({gameOption.MinNumberOfPlayers}-{gameOption.MaxNumberOfPlayers})?";
                IEnumerable<int> options = Enumerable.Range(gameOption.MinNumberOfPlayers, gameOption.MaxNumberOfPlayers - gameOption.MinNumberOfPlayers);
                var playersToAdd = _playerInputProvider.GetPlayerInputAsInt(question, options);
                _gameRunner.ManualGame(playersToAdd);
                break;

            case 4:
                _gameEventHandler.SetMinLevel(EventLevel.Info);

                _gameRunner.ShowStatistics();
                break;
        }
    }

    private IGameRunner GetGameRunner(GameOption gameOption)
    {
        return _gameRunnerFactory.Create(gameOption.Type);
    }

    private GameOption GetUsersSelectedGameOption()
    {
        string question = "Which game do you want to play?\n";
        string options = string.Join("\n", _gameoptions.Select((g, i) => $"{i + 1}. {g.Name}"));
        var message = question + options;

        // REFACTOR - give a IEnumerble<MenuOption> MenuOption(string name, Type option) and print name and return option 
        int gameChoice = _playerInputProvider.GetPlayerInputAsInt(message, Enumerable.Range(1, _gameoptions.Count).ToArray());
        return _gameoptions[gameChoice - 1]; // Note: zero-based index
    }
}
