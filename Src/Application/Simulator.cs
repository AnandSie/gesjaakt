using Domain.Interfaces;
using System.Text;
using Extensions;
using System.Diagnostics;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application;

public class Simulator : ISimulator
{
    private readonly ILogger<Simulator> _simulatorLogger;
    readonly IPlayerFactory _playerFactory;
    readonly IGameDealerFactory _dealerFactory;

    public Simulator(ILogger<Simulator> playerLogger,
        IPlayerFactory playerFactory,
        IGameDealerFactory dealerFactory)
    {
        this._simulatorLogger = playerLogger;
        this._playerFactory = playerFactory;
        this._dealerFactory = dealerFactory;
    }

    public void Start(int numberOfSimulations)
    {
        // Primitive obsession: use PlayerName
        var resultPerPlayer = new Dictionary<string, int>();
        foreach (var iter in Enumerable.Range(1, numberOfSimulations))
        {
            var demoPlayers = _playerFactory.Create().Shuffle();

            _simulatorLogger.LogInformation($"------------- Game #{iter} ---------------- ");
            RunSingleGame(resultPerPlayer, demoPlayers);
        }

        ReportSimulationResults(resultPerPlayer);
    }

    private void RunSingleGame(Dictionary<string, int> resultPerPlayer, IEnumerable<IGesjaaktPlayer> demoPlayers)
    {
        var dealer = _dealerFactory.Create(demoPlayers);
        dealer.Prepare();
        dealer.Play();

        SaveGameResults(resultPerPlayer, dealer);
        ReportGameResults(dealer);
    }

    private static void SaveGameResults(Dictionary<string, int> resultPerPlayer, IGameDealer<IGesjaaktPlayerState> dealer)
    {
        var winner = dealer.Winner();
        if (resultPerPlayer.ContainsKey(winner.Name))
        {
            resultPerPlayer[winner.Name] += 1;
        }
        else
        {
            resultPerPlayer[winner.Name] = 1;
        }
    }


    public void StartAllCombis()
    {
        // TODO: make dynamic or get from method parameter
        var numberOfSimulations = 1000;

        var allPlayerFactories = _playerFactory.AllPlayerFactories().ToList();
        var maxPlayers = 7; // TODO: extract from rule object

        var stopwatch = new Stopwatch();
        double totalElapsedMs = 0;

        var resultPerPlayer = new Dictionary<string, int>();
        var allCombinations = GetCombinations<Func<IGesjaaktPlayer>>(allPlayerFactories, maxPlayers).ToList();
        int numberOfCombinations = allCombinations.Count;
        for (int i = 0; i < numberOfCombinations; i++)
        {
            stopwatch.Restart();

            _simulatorLogger.LogCritical($"------------- Player Combi #{i}/{numberOfCombinations} - {(i/ numberOfCombinations) * 100}% ---------------- ");
            var playerCombi = allCombinations[i].Select(pf => pf.Invoke()).Shuffle();

            foreach (var iter in Enumerable.Range(1, numberOfSimulations))
            {
                _simulatorLogger.LogInformation($"------------- Game #{iter} ---------------- ");

                // TODO: check if we are creating new players (probably not because only call playerfactory once.., so we maybe need to reset them?
                RunSingleGame(resultPerPlayer, playerCombi);
            }

            stopwatch.Stop();
            double elapsed = stopwatch.Elapsed.TotalMilliseconds;
            totalElapsedMs += elapsed;

            stopwatch.Stop();
            int remainingIterations = numberOfCombinations - (i + 1);
            double averageMs = totalElapsedMs / (i + 1);
            double estimatedRemainingMs = averageMs * remainingIterations;

            _simulatorLogger.LogCritical($"Iteration {i + 1}/{numberOfCombinations} took {elapsed:F2} ms. Estimated remaining: {estimatedRemainingMs / 1000/60:F2} min");
        }

        ReportSimulationResults(resultPerPlayer);
    }


    private void ReportGameResults(IGameDealer<IGesjaaktPlayerState> dealer)
    {
        var winner = dealer.Winner();

        var sbGame = new StringBuilder();
        foreach (var player in dealer.GetPlayerResults())
        {
            sbGame.Append($"\t- {player}\n");
        }

        // TODO: replace logger by a gameEventCaptures -> then infrastructure layer can decide if log/ui/api/etc
        _simulatorLogger.LogWarning($"Winner of this single game: {winner.Name}");
        _simulatorLogger.LogWarning(sbGame.ToString());
        _simulatorLogger.LogWarning($"Game results per Player");
    }

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
        _simulatorLogger.LogCritical($"----------------------------------------------------------");
        _simulatorLogger.LogCritical($"Simulation results per Player");
        _simulatorLogger.LogCritical(sb.ToString());
        _simulatorLogger.LogCritical($"The winner of the simulation: {totalWinner.Key}");
    }
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
