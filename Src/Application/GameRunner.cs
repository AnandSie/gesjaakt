using System.Text;
using System.Diagnostics;
using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using Extensions;
using Application.Interfaces;

namespace Application;

public class GameRunner<TPlayer> : IGameRunner where TPlayer : INamed
{
    private readonly ILogger<GameRunner<TPlayer>> _logger;
    private readonly IPlayerFactory<TPlayer> _playerFactory;
    private readonly IGame<TPlayer> _game;
    private readonly IVisualizer _visualizer;
    private Dictionary<string, int> winsByPlayerName;

    public GameRunner(ILogger<GameRunner<TPlayer>> logger,
        IPlayerFactory<TPlayer> playerFactory,
        IGame<TPlayer> game,
        IVisualizer visualizer
    )
    {
        _logger = logger;
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
            _logger.LogInformation($"------------- Game #{iter} ---------------- ");

            var demoPlayers = _playerFactory.Create().Shuffle();
            RunGameWith(demoPlayers);
        }

        ReportSimulationResults(winsByPlayerName);
    }

    public void ManualGame(int numberOfPlayers)
    {
        var manualPlayers = _playerFactory.CreateManualPlayers(numberOfPlayers);
        RunGameWith(manualPlayers);
    }

    public void SimulateAllPossibleCombis()
    {
        // TODO: make dynamic or get from method parameter
        var numberOfSimulations = 1000;

        var allPlayerFactories = _playerFactory.AllPlayerFactories().ToList();
        var maxPlayers = 7; // TODO: extract from rule object

        var stopwatch = new Stopwatch();
        double totalElapsedMs = 0;

        var allCombinations = GetCombinations(allPlayerFactories, maxPlayers).ToList();
        int numberOfCombinations = allCombinations.Count;
        for (int i = 0; i < numberOfCombinations; i++)
        {
            stopwatch.Restart();

            // TODO: replace log by events
            _logger.LogCritical($"------------- Player Combi #{i}/{numberOfCombinations} - {(i / numberOfCombinations) * 100}% ---------------- ");
            var players = allCombinations[i].Select(pf => pf.Invoke()).Shuffle();

            foreach (var iter in Enumerable.Range(1, numberOfSimulations))
            {
                // TODO: replace log by events
                _logger.LogInformation($"------------- Game #{iter} ---------------- ");

                // TODO: check if we are creating new players (probably not because only call playerfactory once.., so we maybe need to reset them?
                RunGameWith(players);
            }

            stopwatch.Stop();
            double elapsed = stopwatch.Elapsed.TotalMilliseconds;
            totalElapsedMs += elapsed;

            stopwatch.Stop();
            int remainingIterations = numberOfCombinations - (i + 1);
            double averageMs = totalElapsedMs / (i + 1);
            double estimatedRemainingMs = averageMs * remainingIterations;

            _logger.LogCritical($"Iteration {i + 1}/{numberOfCombinations} took {elapsed:F2} ms. Estimated remaining: {estimatedRemainingMs / 1000 / 60:F2} min");
        }

        ReportSimulationResults(winsByPlayerName);
    }

    private void RunGameWith(IEnumerable<TPlayer> players)
    {
        _game.PlayWith(players);
        
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
        var winner = playerResults.First();

        var sbGame = new StringBuilder();
        foreach (var player in playerResults)
        {
            sbGame.Append($"\t- {player}\n");
        }

        // TODO: replace logger by a gameEventCaptures -> then infrastructure layer can decide if log/ui/api/etc
        _logger.LogWarning($"Winner of this single game: {winner.Name}");
        _logger.LogWarning($"Game results per Player");
        _logger.LogWarning(sbGame.ToString());
    }

    // TODO: Voelt dubel op met ReportGameResults
    private void ReportSimulationResults(Dictionary<string, int> resultPerPlayer)
    {
        var numberOfGames = resultPerPlayer.Values.Sum();

        // Don't save wins, save points - or do both
        var simlationResultsOrdened = resultPerPlayer.OrderByDescending(entry => entry.Value);
        var totalWinner = simlationResultsOrdened.FirstOrDefault();

        var sb = new StringBuilder();
        foreach (var player in simlationResultsOrdened)
        {
            sb.AppendLine($"- {player.Key} - {player.Value} wins -  {(double)player.Value / numberOfGames * 100}%");
        }

        var totGames = simlationResultsOrdened.Select(p => p.Value).Sum();
        sb.Append($"Total Games Played {totGames}");
        // TODO: report regarding combinations

        // TODO: replace logger by a gameEventCaptures -> then infrastructure layer can decide if log/ui/api/etc
        _logger.LogCritical($"----------------------------------------------------------");
        _logger.LogCritical($"Simulation results per Player");
        _logger.LogCritical(sb.ToString());
        _logger.LogCritical($"The winner of the simulation: {totalWinner.Key}");
    }

    // REFACTOR: add documentation
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

}
