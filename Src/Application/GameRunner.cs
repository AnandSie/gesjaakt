using System.Text;
using System.Diagnostics;
using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using Extensions;
using Application.Interfaces;
using Domain.Entities.Events;

namespace Application;

public class GameRunner<TPlayer> : IGameRunner where TPlayer : INamed
{
    private readonly IPlayerFactory<TPlayer> _playerFactory;
    private readonly IGame<TPlayer> _game;
    private readonly IStatisticsCreator _visualizer;

    private Dictionary<string, int> winsByPlayerName;

    public event EventHandler<WarningEvent>? GameEnded;
    public event EventHandler<ErrorEvent>? GameSimulationStarting;
    public event EventHandler<ErrorEvent>? GameSimulationEnded;
    public event EventHandler<CriticalEvent>? PlayerCombinationSimulationStarting;
    public event EventHandler<CriticalEvent>? PlayerCombinationSimulationEnded;

    public GameRunner(
        IPlayerFactory<TPlayer> playerFactory,
        IGame<TPlayer> game,
        IStatisticsCreator visualizer
    )
    {
        _playerFactory = playerFactory;
        _game = game;
        _visualizer = visualizer;

        // REFACTOR - Primitive obsession -> string replace by PlayerName
        winsByPlayerName = new Dictionary<string, int>();
    }

    public void ShowStatistics()
    {
        _visualizer.Show();
    }

    public void Simulate(int numberOfSimulations)
    {
        foreach (var iter in Enumerable.Range(1, numberOfSimulations))
        {
            ShareGameSimulationStarting(numberOfSimulations, iter);

            var demoPlayers = _playerFactory.Create().Shuffle();
            RunGameWith(demoPlayers);
        }

        ReportSimulationResults(winsByPlayerName);
    }

    private void ShareGameSimulationStarting(int numberOfSimulations, int iter)
    {
        string message = $"Game Simulation Starting #{iter}/numberOfSimulations -  {(iter / numberOfSimulations) * 100}%";
        GameSimulationStarting?.Invoke(this, new(message));
    }

    public void ManualGame(int numberOfPlayers)
    {
        var manualPlayers = _playerFactory.CreateManualPlayers(numberOfPlayers);
        RunGameWith(manualPlayers);
    }

    public void SimulateAllPossibleCombis()
    {
        // REFACTOR: make dynamic or get from method parameter
        var numberOfSimulations = 1000;

        var allPlayerFactories = _playerFactory.AllPlayerFactories().ToList();
        var maxPlayers = 7; // REFACTOR: extract from rule object

        var allPlayerFactoryCombinations = GetCombinations(allPlayerFactories, maxPlayers).ToList();
        int numberOfCombinations = allPlayerFactoryCombinations.Count;

        var stopwatch = new GameRunnerStopWatch(numberOfSimulations);
        for (int playerCombiIter = 0; playerCombiIter < numberOfCombinations; playerCombiIter++)
        {
            stopwatch.IterationStart();

            string startMessage = $"Player Combination Starting #{playerCombiIter}/{numberOfCombinations} - {(playerCombiIter / numberOfCombinations) * 100}%";
            PlayerCombinationSimulationStarting?.Invoke(this, new(startMessage));

            var players = allPlayerFactoryCombinations[playerCombiIter].Select(pf => pf.Invoke()).Shuffle();
            foreach (var simIter in Enumerable.Range(1, numberOfSimulations))
            {
                ShareGameSimulationStarting(numberOfSimulations, simIter);
                RunGameWith(players);
            }

            stopwatch.IterationFinished();
            var estimatedRemainingMinutes = stopwatch.EstimatedRemainingMinutes(playerCombiIter);

            string endMessage = $"Player Combination Ended. It took {stopwatch.Elapsed():F2} ms. Estimated remaining time: {estimatedRemainingMinutes:F2} min";
            PlayerCombinationSimulationEnded?.Invoke(this, new(endMessage));
        }

        ReportSimulationResults(winsByPlayerName);
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

        // REFACTOR - (Gesjaakt specific) Don't save wins, save points - or do both
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

        GameSimulationEnded?.Invoke(this, new(message.ToString()));
    }

    // REFACTOR: add documentation
    // Q: wat doet ? moet er ook niet een soort minimum zijn?
    private static IEnumerable<IEnumerable<T>> GetCombinations<T>(List<T> source, int k)
    {
        if (k == 0)
        {
            yield return Enumerable.Empty<T>();
        }
        else
        {
            for (int i = 0; i <= source.Count - k; i++)
            {
                foreach (var tail in GetCombinations(source.Skip(i + 1).ToList(), k - 1).ToList())
                {
                    yield return new[] { source[i] }.Concat(tail);
                }
            }
        }
    }

    private class GameRunnerStopWatch(int numberOfIterations)
    {
        private double totalElapsedMs = 0;
        private readonly Stopwatch stopwatch = new();

        public void IterationStart()
        {
            stopwatch.Restart();
        }

        public void IterationFinished()
        {
            stopwatch.Stop();
        }

        public double EstimatedRemainingMinutes(int finishedIteration)
        {
            totalElapsedMs += Elapsed();
            return RemainingTimeMs(totalElapsedMs, numberOfIterations, finishedIteration) / 1000 / 60;
        }

        public double Elapsed()
        {
            return stopwatch.Elapsed.TotalMilliseconds;
        }


        private static double RemainingTimeMs(double totalElapsedMs, int totalIteration, int finishedIteration)
        {
            int remainingIterations = totalIteration - (finishedIteration + 1);
            double averageMsPerIteration = totalElapsedMs / (finishedIteration + 1);

            return averageMsPerIteration * remainingIterations;
        }
    }
}
