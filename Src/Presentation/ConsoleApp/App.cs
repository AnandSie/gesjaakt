using Application;
using Application.Interfaces;
using Domain.Entities.Events;
using UserInterface;
using WinFormsApplication = System.Windows.Forms.Application;

namespace ConsoleApp;

internal class App
{
    private readonly List<GameOption> _gameoptions;
    private readonly IPlayerInputProvider _playerInputProvider;
    private readonly GameRunnerFactory _gameRunnerFactory;
    private readonly IGameRunnerEventCollector _gameEventCollector;
    private readonly IGameEventHandler _gameEventHandler;
    private readonly SimulationConfiguration _simulationConfiguration;
    private readonly WidgetDisplay _display;

    public App(
        List<GameOption> gameoptions,
        IPlayerInputProvider playerInputProvider,
        GameRunnerFactory gameRunnerFactory,
        IGameRunnerEventCollector gameEventCollector,
        IGameEventHandler gameEventHandler,
        SimulationConfiguration simulationConfiguration,
        WidgetDisplay display)
    {
        _gameoptions = gameoptions;
        _playerInputProvider = playerInputProvider;
        _gameRunnerFactory = gameRunnerFactory;
        _gameEventCollector = gameEventCollector;
        _gameEventHandler = gameEventHandler;
        _simulationConfiguration = simulationConfiguration;
        _display = display;

        var thread = new Thread(() =>
        {
            WinFormsApplication.EnableVisualStyles();
            WinFormsApplication.SetCompatibleTextRenderingDefault(false);
            WinFormsApplication.Run(_display);
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }

    public void Start()
    {
        var gameOption = SelectGame();
        IGameRunner _gameRunner = _gameRunnerFactory.Create(gameOption.Type);
        _gameEventCollector.Attach(_gameRunner);
        SelectGameMode(gameOption, _gameRunner);
    }

    private void SelectGameMode(GameOption gameOption, IGameRunner _gameRunner)
    {
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
                _gameRunner.Simulate(_simulationConfiguration.NumberOfGamesPerSimulation);
                break;

            case 2:
                _gameEventHandler.SetMinLevel(EventLevel.Critical);
                _gameRunner.SimulateAllPossiblePlayerCombis();
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

    private GameOption SelectGame()
    {
        string question = "Which game do you want to play?\n";
        string options = string.Join("\n", _gameoptions.Select((g, i) => $"{i + 1}. {g.Name}"));
        var message = question + options;

        // REFACTOR - give a IEnumerble<MenuOption> MenuOption(string name, Type option) and print name and return option 
        int gameChoice = _playerInputProvider.GetPlayerInputAsInt(message, Enumerable.Range(1, _gameoptions.Count).ToArray());
        return _gameoptions[gameChoice - 1]; // Note: zero-based index
    }
}
