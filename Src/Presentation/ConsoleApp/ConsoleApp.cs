using Application;
using Application.Interfaces;
using Domain.Interfaces;

namespace ConsoleApp;

internal class ConsoleApp
{
    private readonly ILogger<ConsoleApp> _logger;
    private readonly List<Option> _gameoptions;
    private readonly IPlayerInputProvider _playerInputProvider;
    private readonly GameRunnerFactory _gameRunnerFactory;
    private readonly IGameRunnerEventCollector _gameEventCollector;

    public ConsoleApp(ILogger<ConsoleApp> logger,
        List<Option> gameoptions,
        IPlayerInputProvider playerInputProvider,
        GameRunnerFactory gameRunnerFactory,
        IGameRunnerEventCollector gameEventCollector)
    {
        _logger = logger;
        _gameoptions = gameoptions;
        _playerInputProvider = playerInputProvider;
        _gameRunnerFactory = gameRunnerFactory;
        _gameEventCollector = gameEventCollector;
    }

    public void Start()
    {
        IGameRunner _gameRunner = GetGameRunner();
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

    private IGameRunner GetGameRunner()
    {
        string question = "Which game do you want to play?\n";
        string options = string.Join("\n", _gameoptions.Select((g, i) => $"{i + 1}. {g.Name}"));
        var message = question + options;

        // REFACTOR - give a IEnumerble<MenuOption> MenuOption(string name, Type option) and print name and return option 
        int gameChoice = _playerInputProvider.GetPlayerInputAsInt(message, Enumerable.Range(1, _gameoptions.Count).ToArray());
        var selected = _gameoptions[gameChoice - 1]; // Note: zero-based index

        return _gameRunnerFactory.Create(selected.Type);
    }
}
