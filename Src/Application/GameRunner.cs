using System.Text;
using Application.Interfaces;
using Domain.Entities.Events;
using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using Extensions;

namespace Application;

public class GameRunner<TPlayer> : IGameRunner where TPlayer : INamed
{
    private readonly IPlayerFactory<TPlayer> _playerFactory;
    private readonly IGame<TPlayer> _game;
    private readonly IStatisticsCreator _visualizer;
    private readonly SimulationConfiguration _config;

    private Dictionary<string, int> winsByPlayerName;

    public event EventHandler<WarningEvent>? GameEnded;
    public event EventHandler<ErrorEvent>? SimIterStarting;
    public event EventHandler<WarningEvent>? SimIterEnded;
    public event EventHandler<ErrorEvent>? AllSimItersEnded;
    public event EventHandler<CriticalEvent>? PlayerCombinationIterStarting;
    public event EventHandler<CriticalEvent>? PlayerCombinationIterEnded;

    public GameRunner(
        IPlayerFactory<TPlayer> playerFactory,
        IGame<TPlayer> game,
        IStatisticsCreator visualizer,
        SimulationConfiguration config
    )
    {
        _playerFactory = playerFactory;
        _game = game;
        _visualizer = visualizer;
        _config = config;

        // REFACTOR - Primitive obsession -> string replace by PlayerName
        winsByPlayerName = new Dictionary<string, int>();
    }
    
    public void ManualGame(int numberOfPlayers)
    {
        var manualPlayers = _playerFactory.CreateManualPlayers(numberOfPlayers);
        RunGameWith(manualPlayers);
    }

    public void ShowStatistics()
    {
        _visualizer.Show();
    }

    public void Simulate(int numberOfSimulations)
    {
        var desiredDuration = TimeSpan.FromSeconds(_config.TargetSimulationDurationSeconds);
        var stopwatch = new LoopStopWatch(numberOfSimulations, desiredDuration);

        // REFACTOR - parallel
        foreach (var iter in Enumerable.Range(1, numberOfSimulations))
        {
            stopwatch.IterationHasStarted();
            ShareGameSimIterStarting(numberOfSimulations, iter);

            var players = _playerFactory.Create().Shuffle();
            RunGameWith(players);
            ShareSimulationEnded();

            stopwatch.IterationHasFinished();
            stopwatch.SleepToMatchTargetTime();
        }

        ReportSimulationResults(winsByPlayerName);
    }


    public void SimulateAllPossiblePlayerCombis()
    {
        var allPlayerFactories = _playerFactory.AllPlayerFactories().ToList();
        var allPlayerFactoryCombinations = GetCombinations(allPlayerFactories, _game.MaxNumberOfPlayers).ToList();

        int numberOfPlayerCombinations = allPlayerFactoryCombinations.Count;
        var numberOfSimulations = _config.NumberOfSimulationsPerPlayerCombination;
        var stopwatch = new LoopStopWatch(numberOfSimulations);
        // REFACTOR - parallel
        for (int i = 0; i < numberOfPlayerCombinations; i++)
        {
            stopwatch.IterationHasStarted();
            string startMessage = $"Player Combination Starting #{i}/{numberOfPlayerCombinations} - {(i / numberOfPlayerCombinations) * 100}%";
            PlayerCombinationIterStarting?.Invoke(this, new(startMessage));

            var players = allPlayerFactoryCombinations[i].Select(pf => pf.Invoke()).Shuffle();

            // REFACTOR - reuse the public simulate
            foreach (var simIter in Enumerable.Range(1, numberOfSimulations))
            {
                ShareGameSimIterStarting(numberOfSimulations, simIter);
                RunGameWith(players.Shuffle());
                ShareSimulationEnded();
            }

            stopwatch.IterationHasFinished();
            var estimatedRemainingMinutes = stopwatch.RemainingMinutes();

            string endMessage = $"Player Combination Ended. It took {stopwatch.IterationDurationSec()} s. Estimated remaining time: {estimatedRemainingMinutes:F2} min";
            PlayerCombinationIterEnded?.Invoke(this, new(endMessage));
        }

        ReportSimulationResults(winsByPlayerName);
    }

    private void ShareSimulationEnded()
    {
        var sb = new StringBuilder();
        var maxNameLength = winsByPlayerName.Keys.Select(name => name.Length).Max();
        const int maxWinsLength = 5; // REFACTOR: make dynamic 

        var totalWinsSoFar = winsByPlayerName.Values.Sum();
        foreach (var kv in winsByPlayerName.OrderByDescending(kvp => kvp.Value))
        {
            double percentage = (double)kv.Value / totalWinsSoFar * 100;
            sb.AppendLine($"{kv.Key.PadRight(maxNameLength)} - {kv.Value,maxWinsLength} wins - {percentage,2:F0}%");
        }

        var msg = sb.ToString();
        SimIterEnded?.Invoke(this, new(msg));
    }

    private void ShareGameSimIterStarting(int numberOfSimulations, int iter)
    {
        double progress = (double)iter / numberOfSimulations * 100;
        string message = $"Game Simulation Starting #{iter}/{numberOfSimulations} -  {progress:F0}%";
        SimIterStarting?.Invoke(this, new(message));
    }

    private void RunGameWith(IEnumerable<TPlayer> players)
    {
        _game.PlayWith(players);

        // REFACTOR - Create seperate GameResult Object where stuff like winner is in - problem now is that we are calculating winner twice (dangerous). Maybe we want to share some other statistics like amount of events happened
        var playerResults = _game.Results();
        ReportGameResults(playerResults);

        var winner = playerResults.First();
        SaveGameResults(winner);
    }

    private void SaveGameResults(TPlayer winner)
    {
        winsByPlayerName[winner.Name] = winsByPlayerName.GetValueOrDefault(winner.Name) + 1;
    }

    private void ReportGameResults(IOrderedEnumerable<TPlayer> playerResults)
    {
        var message = new StringBuilder();
        message.AppendLine($"Game Winner: {playerResults.First().Name}");
        message.AppendLine($"Results");
        foreach (var player in playerResults)
        {
            message.AppendLine($"\t- {player}");
        }

        GameEnded?.Invoke(this, new(message.ToString()));
    }

    private void ReportSimulationResults(Dictionary<string, int> resultPerPlayer)
    {
        var numberOfGames = resultPerPlayer.Values.Sum();

        // REFACTOR - (Game Gesjaakt specific) Don't save wins, save points - or do both
        var simlationResultsOrdened = resultPerPlayer.OrderByDescending(entry => entry.Value);
        var winner = simlationResultsOrdened.FirstOrDefault().Key;

        var message = new StringBuilder();
        message.AppendLine($"Simulation Winner: {winner}");
        message.AppendLine($"Results");
        foreach (var player in simlationResultsOrdened)
        {
            message.AppendLine($"\t- {player.Key} - {player.Value} wins - {(double)player.Value / numberOfGames * 100}%");
        }
        var gamesPlayed = simlationResultsOrdened.Select(p => p.Value).Sum();
        message.AppendLine($"Total Games Played {gamesPlayed}");

        AllSimItersEnded?.Invoke(this, new(message.ToString()));
    }

    // REFACTOR: add documentation
    private static IEnumerable<IEnumerable<T>> GetCombinations<T>(List<T> source, int maxPerCombi)
    {
        if (source.Count < maxPerCombi)
        {
            throw new ArgumentException($"There are less source elements {source.Count} than is requested per combination {maxPerCombi}");
        }

        if (maxPerCombi == 0)
        {
            yield return Enumerable.Empty<T>();
        }
        else
        {
            for (int i = 0; i <= source.Count - maxPerCombi; i++)
            {
                foreach (var tail in GetCombinations(source.Skip(i + 1).ToList(), maxPerCombi - 1).ToList())
                {
                    yield return new[] { source[i] }.Concat(tail);
                }
            }
        }
    }
}
