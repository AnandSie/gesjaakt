using Application;
using Domain.Entities.Players;
using Domain.Interfaces;
using Extensions;
using System.Text;
using Visualization;

namespace ConsoleApp;

internal class App
{
    readonly ILogger<App> _logger;
    readonly IPlayerFactory _playerFactory;
    readonly IPlayerInputProvider _playerInputProvider;
    readonly IGameDealerFactory _dealerFactory;

    public App(ILogger<App> logger, IPlayerFactory playerFactory, IPlayerInputProvider playerInputProvider, IGameDealerFactory dealerFactory)
    {
        _logger = logger;
        _playerFactory = playerFactory;
        _playerInputProvider = playerInputProvider;
        _dealerFactory = dealerFactory;
    }

    public void Run()
    {
        _logger.LogCritical(
            """
            LETS PLAY!
            What do you want?
            1. Simulated Game
            2. Manual Game
            3. Visualize a thinker
            """
                );

        var choice = _playerInputProvider.GetPlayerInputAsInt(new[] { 1, 2, 3 });


        // TODO: Extract class
        switch (choice)
        {
            case 1:
                // TODO: Log critical
                var simsAmount = _playerInputProvider.GetPlayerInputAsIntWithMinMax("How often should we run it", 1, 10000);
                _logger.LogCritical($"Simulation started with {simsAmount} runs");
                var resultPerPlayer = new Dictionary<string, int>(); // Primitive obsession: use PlayerName

                foreach (var iter in Enumerable.Range(1, simsAmount))
                {
                    _logger.LogInformation($"----------------------------- iter #{iter}");
                    var demoPlayers = _playerFactory.Create().Shuffle();
                    var dealer = _dealerFactory.Create(demoPlayers);
                    dealer.Play();
                    var winner = dealer.Winner();

                    // TODO: EXTRACT METHOD FOR GAMEDEALER -> USE DEPENDENCY INJECTION FOR WHERE TO PRINT/SHARE
                    var playerList = demoPlayers
                        .OrderBy(p => p.CardPoints() - p.CoinsAmount)
                        .Select(p => $"\t- {p}")
                        .Aggregate((current, next) => current + Environment.NewLine + next);

                    _logger.LogWarning($"Winner of this round: {winner.Name}\n{playerList}");

                    if (resultPerPlayer.ContainsKey(winner.Name))
                    {
                        resultPerPlayer[winner.Name] += 1;
                    }
                    else
                    {
                        resultPerPlayer[winner.Name] = 1;
                    }
                }

                var totalWinner = resultPerPlayer.OrderByDescending(entry => entry.Value).FirstOrDefault();

                _logger.LogCritical($"-----------------------------\n\t" +
                                    $"The winner is: {totalWinner.Key}");

                var sb = new StringBuilder();
                foreach (var player in resultPerPlayer.OrderByDescending(entry => entry.Value))
                {
                    sb.AppendLine($"- {player.Key}, {(double)player.Value / simsAmount * 100}%");
                }
                _logger.LogCritical(sb.ToString());

                _logger.LogCritical($"Press enter to exit");
                Console.ReadLine();
                break;

            case 2:
                var playersToAdd = _playerInputProvider.GetPlayerInputAsInt("How many players do you want to play (3-7)?", new[] { 3, 4, 5, 6, 7 });

                var players = new List<Player>();
                foreach (var i in Enumerable.Range(3, playersToAdd))
                {
                    var name = _playerInputProvider.GetPlayerInput($"Player number {i - 2} what is your name?");
                    players.Add((Player)_playerFactory.CreateHomoSapiens(name, _playerInputProvider));
                }

                var dealerManualGame = _dealerFactory.Create(players);
                dealerManualGame.Play();
                var winnerManualGame = dealerManualGame.Winner();

                var logMessage = new StringBuilder();

                logMessage.AppendLine("-----------------------------");
                logMessage.AppendLine($"\tThe winner is: {winnerManualGame.Name}");

                foreach (var player in players.OrderBy(p => p.CardPoints() - p.CoinsAmount))
                {
                    logMessage.AppendLine($"\t- {player}");
                }

                _logger.LogCritical(logMessage.ToString());
                _logger.LogCritical($"Press enter to exit");
                Console.ReadLine();
                break;

            case 3:
                new Visualizer().Show();
                break;
        }
    }
}
