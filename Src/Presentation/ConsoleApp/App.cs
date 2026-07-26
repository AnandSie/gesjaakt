using Application;
using Application.Interfaces;
using Domain.Entities.Events;

namespace ConsoleApp;

internal class App
{
    private readonly List<GameOption> _gameoptions;
    private readonly IPlayerInputProvider _playerInputProvider;
    private readonly GameRunnerFactory _gameRunnerFactory;
    private readonly IGameRunnerEventCollector _gameEventCollector;
    private readonly IGameEventHandler _gameEventHandler;
    private readonly SimulationConfiguration _simulationConfiguration;

    public App(
        List<GameOption> gameoptions,
        IPlayerInputProvider playerInputProvider,
        GameRunnerFactory gameRunnerFactory,
        IGameRunnerEventCollector gameEventCollector,
        IGameEventHandler gameEventHandler,
        SimulationConfiguration simulationConfiguration)
    {
        _gameoptions = gameoptions;
        _playerInputProvider = playerInputProvider;
        _gameRunnerFactory = gameRunnerFactory;
        _gameEventCollector = gameEventCollector;
        _gameEventHandler = gameEventHandler;
        _simulationConfiguration = simulationConfiguration;
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
                // Only the rare stuff is narrated during a simulation; everything
                // else is still recorded and shows up in the summary afterwards.
                _gameEventHandler.SetMinImportance(EventImportance.Special);
                _gameRunner.Simulate(_simulationConfiguration.NumberOfGamesPerSimulation);
                _gameEventHandler.ShowSummary();
                break;

            case 2:
                _gameEventHandler.SetMinImportance(EventImportance.GameChanging);
                _gameRunner.SimulateAllPossiblePlayerCombis();
                _gameEventHandler.ShowSummary();
                break;

            case 3:
                _gameEventHandler.SetMinImportance(EventImportance.Ordinary);

                string question = $"With how many players do you want to play ({gameOption.MinNumberOfPlayers}-{gameOption.MaxNumberOfPlayers})?";
                // Range's second argument is a count, not an end value - without the +1 the
                // maximum is never offered, so the question above advertised a player count
                // (e.g. 5 for Qwixx, 7 for Gesjaakt, 10 for Take-5!) that was then rejected.
                IEnumerable<int> options = Enumerable.Range(gameOption.MinNumberOfPlayers, gameOption.MaxNumberOfPlayers - gameOption.MinNumberOfPlayers + 1);
                var playersToAdd = _playerInputProvider.GetPlayerInputAsInt(question, options);
                _gameRunner.ManualGame(playersToAdd);
                _gameEventHandler.ShowSummary();
                break;

            case 4:
                _gameEventHandler.SetMinImportance(EventImportance.Ordinary);

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
