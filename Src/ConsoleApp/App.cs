using ConsoleApp.Helpers;
using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Entities.Thinkiers;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application;
using Extensions;

namespace ConsoleApp;

internal class App
{
    readonly ILogger logger;
    readonly IPlayerFactory PlayerFactory;

    public App(ILogger<App> logger, IPlayerFactory playerFactory)
    {
        this.logger = logger;
        this.PlayerFactory = playerFactory;
    }

    public void Run()
    {

        Console.WriteLine("LETS PLAY!");
        Console.WriteLine("What do you want?");
        Console.WriteLine("1. Simulated Game");
        Console.WriteLine("2. Manual Game");

        var choice = new ConsoleInputService(logger).GetPlayerInputAsInt(new[] { 1, 2 });


        // TODO: Extract class
        switch (choice)
        {
            case 1:
                // TODO: Log critical
                var simsAmount = new ConsoleInputService(logger).GetPlayerInputAsIntWithMinMax("How often should we run it", 1, 10000);
                logger.LogInformation($"Simulation started with {simsAmount} runs");
                var resultPerPlayer = new Dictionary<string, int>(); // Primitive obsession: use PlayerName

                foreach (var iter in Enumerable.Range(1, simsAmount))
                {
                    logger.LogInformation("-----------------------------");
                    logger.LogInformation($"iter #{iter}");
                    var demoPlayers = PlayerFactory.Create().Shuffle();
                    var dealer = new GameDealer(demoPlayers);
                    dealer.Play();
                    var winner = dealer.Winner();

                    // TODO: EXTRACT METHOD FOR GAMEDEALER -> USE DEPENDENCY INJECTION FOR WHERE TO PRINT/SHARE
                    logger.LogInformation($"Winner of this round: {winner.Name}");
                    foreach (var player in demoPlayers.OrderBy(p => p.CardPoints() - p.CoinsAmount))//FIXME: create player method/value
                    {
                        logger.LogInformation($"- {player}");
                    }

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

                logger.LogCritical("-----------------------------");
                logger.LogCritical($"The winner is: {totalWinner.Key}");

                var sb = new StringBuilder();
                foreach (var player in resultPerPlayer.OrderByDescending(entry => entry.Value))
                {
                    sb.AppendLine($"- {player.Key}, {(double)player.Value / simsAmount * 100}%");
                }
                logger.LogCritical(sb.ToString());

                break;

            case 2:
                var playersToAdd = new ConsoleInputService(logger).GetPlayerInputAsInt("How many players do you want to play (3-7)?", new[] { 3, 4, 5, 6, 7 });

                var players = new List<Player>();
                foreach (var i in Enumerable.Range(3, playersToAdd))
                {
                    var name = new ConsoleInputService(logger).GetPlayerInput($"Player number {i - 2} what is your name?");
                    players.Add(new Player(new HomoSapiensThinker(new ConsoleInputService(logger), name), name)); // FIXME Both classes uses name
                }

                var dealerManualGame = new GameDealer(players);
                dealerManualGame.Play();
                var winnerManualGame = dealerManualGame.Winner();

                Console.WriteLine("-------------");
                Console.WriteLine($"The winner is {winnerManualGame.Name}\n");

                foreach (var player in players.OrderBy(p => p.CardPoints() - p.CoinsAmount))//FIXME: create player method/value
                {
                    Console.WriteLine($"- {player}");
                }
                break;
        }
    }
}
