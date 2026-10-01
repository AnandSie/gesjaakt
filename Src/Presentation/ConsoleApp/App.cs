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
    private readonly IOptionsChooserService _optionsChooserService;

    private IGameRunner _gameRunner;
    private GameOption _selectedGameOption;


    public App(
        List<GameOption> gameoptions,
        IPlayerInputProvider playerInputProvider,
        GameRunnerFactory gameRunnerFactory,
        IGameRunnerEventCollector gameEventCollector,
        IGameEventHandler gameEventHandler,
        SimulationConfiguration simulationConfiguration,
        IOptionsChooserService optionsChooserService)
    {
        _gameoptions = gameoptions;
        _playerInputProvider = playerInputProvider;
        _gameRunnerFactory = gameRunnerFactory;
        _gameEventCollector = gameEventCollector;
        _gameEventHandler = gameEventHandler;
        _simulationConfiguration = simulationConfiguration;
        _optionsChooserService = optionsChooserService;
    }

    public void Start()
    {
        _selectedGameOption = _optionsChooserService.ChoiceFromPlayer("Which game do you want to play?", _gameoptions);
        _gameRunner = _gameRunnerFactory.Create(_selectedGameOption.Type);
        _gameEventCollector.Attach(_gameRunner);
        SelectedGameMode().Invoke();
    }

    private Action SelectedGameMode()
    {
        var options = new[]
        {
            new ActionOption("Simulate a set of games", SimulateSingleGame),
            new ActionOption("Simulate all possible player combinations", SimulateAllPossibleGames),
            new ActionOption("Play a manual game", PlayManualGame),
            new ActionOption("Visualize a thinker", VisualizeThinker)
        };

        return _optionsChooserService.ChoiceFromPlayer("What do you want to do?", options).Action;
    }

    private void VisualizeThinker()
    {
        _gameEventHandler.SetMinImportance(EventImportance.Ordinary);

        _gameRunner.ShowStatistics();
    }

    private void PlayManualGame()
    {
        _gameEventHandler.SetMinImportance(EventImportance.Ordinary);

        string question = $"With how many players do you want to play ({_selectedGameOption.MinNumberOfPlayers}-{_selectedGameOption.MaxNumberOfPlayers})?";
        // Range's second argument is a count, not an end value - without the +1 the
        // maximum is never offered, so the question above advertised a player count
        // (e.g. 5 for Qwixx, 7 for Gesjaakt, 10 for Take-5!) that was then rejected.
        IEnumerable<int> options = Enumerable.Range(_selectedGameOption.MinNumberOfPlayers, _selectedGameOption.MaxNumberOfPlayers - _selectedGameOption.MinNumberOfPlayers + 1);
        var playersToAdd = _playerInputProvider.GetPlayerInputAsInt(question, options);
        _gameRunner.StartManualGame(playersToAdd);
        _gameEventHandler.ShowSummary();
    }

    private void SimulateAllPossibleGames()
    {
        _gameEventHandler.SetMinImportance(EventImportance.GameChanging);
        _gameRunner.StartAllPossiblePlayerCombinationSimulation();
        _gameEventHandler.ShowSummary();
    }

    private void SimulateSingleGame()
    {
        // Only the rare stuff is narrated during a simulation; everything
        // else is still recorded and shows up in the summary afterwards.
        _gameEventHandler.SetMinImportance(EventImportance.Special);
        _gameRunner.StartSingleSimulation(_simulationConfiguration.NumberOfGamesPerSimulation);
        _gameEventHandler.ShowSummary();
    }
}
