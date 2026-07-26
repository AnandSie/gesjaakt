using System.Text;
using Application.Interfaces;
using Domain.Entities.Events;
using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using Extensions;

namespace Application;

public class GameRunner<TPlayer> : IGameRunner where TPlayer : INamed, IScored
{
    private readonly IPlayerFactory<TPlayer> _playerFactory;
    private readonly IGame<TPlayer> _game;
    private readonly IStatisticsCreator _visualizer;
    private readonly SimulationConfiguration _config;

    private Dictionary<string, int> winsByPlayerName;

    public event EventHandler<NotableEvent>? GameEnded;
    public event EventHandler<SpecialEvent>? SimIterStarting;
    public event EventHandler<NotableEvent>? SimIterEnded;
    public event EventHandler<GameChangingEvent>? AllSimItersEnded;
    public event EventHandler<GameChangingEvent>? PlayerCombinationIterStarting;
    public event EventHandler<GameChangingEvent>? PlayerCombinationIterEnded;

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
            PlayerCombinationIterStarting?.Invoke(this, new(nameof(PlayerCombinationIterStarting), startMessage, EventCategory.Progress));

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
            PlayerCombinationIterEnded?.Invoke(this, new(nameof(PlayerCombinationIterEnded), endMessage, EventCategory.Progress));
        }

        ReportSimulationResults(winsByPlayerName);
    }

    private void ShareSimulationEnded()
    {
        var msg = string.Join(Environment.NewLine, Standings());
        SimIterEnded?.Invoke(this, new(nameof(SimIterEnded), msg, EventCategory.Progress));
    }

    // One standings format, used both for the live block during a run and for the
    // final results, so the numbers don't change shape when the run ends.
    private IEnumerable<string> Standings()
    {
        var maxNameLength = winsByPlayerName.Keys.Select(name => name.Length).Max();
        const int maxWinsLength = 5; // REFACTOR: make dynamic
        var totalWins = winsByPlayerName.Values.Sum();

        return winsByPlayerName
            .OrderByDescending(entry => entry.Value)
            .Select(entry =>
            {
                double percentage = (double)entry.Value / totalWins * 100;
                return $"{entry.Key.PadRight(maxNameLength)} - {entry.Value,maxWinsLength} wins - {percentage,4:F1}%";
            });
    }

    private void ShareGameSimIterStarting(int numberOfSimulations, int iter)
    {
        double progress = (double)iter / numberOfSimulations * 100;
        string message = $"Game Simulation Starting #{iter}/{numberOfSimulations} -  {progress:F0}%";
        SimIterStarting?.Invoke(this, new(nameof(SimIterStarting), message, EventCategory.Progress));
    }

    private void RunGameWith(IEnumerable<TPlayer> players)
    {
        _game.PlayWith(players.Shuffle());

        // REFACTOR - Create seperate GameResult Object where stuff like winner is in - problem now is that we are calculating winner twice (dangerous). Maybe we want to share some other statistics like amount of events happened
        var playerResults = _game.Results();
        ReportGameResults(playerResults);

        // REFACOR - maak netter, is wat gebeund
        var winner = playerResults.First();
        var winners = playerResults.Where(p => p.Score == winner.Score).ToList();
        winners.ForEach(SaveGameResults);
    }

    private void SaveGameResults(TPlayer winner)
    {
        winsByPlayerName[winner.Name] = winsByPlayerName.GetValueOrDefault(winner.Name) + 1;
    }

    private void ReportGameResults(IOrderedEnumerable<TPlayer> playerResults)
    {
        var message = new StringBuilder();

        // First line is the headline the presenter promotes to the box title.
        message.AppendLine($"Game Winner: {playerResults.First().Name}");
        foreach (var player in playerResults)
        {
            message.AppendLine($"{player}");
        }

        GameEnded?.Invoke(this, new(nameof(GameEnded), message.ToString(), EventCategory.Result));
    }

    private void ReportSimulationResults(Dictionary<string, int> resultPerPlayer)
    {
        // REFACTOR - (Game Gesjaakt specific) Don't save wins, save points - or do both
        var winner = resultPerPlayer.OrderByDescending(entry => entry.Value).FirstOrDefault().Key;
        var gamesPlayed = resultPerPlayer.Values.Sum();

        var message = new StringBuilder();

        // First line is the headline the presenter promotes to the box title.
        message.AppendLine($"Simulation Winner: {winner}");
        foreach (var line in Standings())
        {
            message.AppendLine(line);
        }
        message.AppendLine($"Total Games Played {gamesPlayed}");

        AllSimItersEnded?.Invoke(this, new(nameof(AllSimItersEnded), message.ToString(), EventCategory.Result));
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
