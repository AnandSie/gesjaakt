using ConsoleApp.Helpers;
using Domain.Entities.Game;
using Domain.Entities.Players;
using Domain.Interfaces;

Console.WriteLine("LETS PLAY!");
Console.WriteLine("What do you want?");
Console.WriteLine("1. Demo Game");
Console.WriteLine("2. Custom Game");

// TODO: IOC
var choice = new ConsoleInputService().GetPlayerInputAsInt(new[] { 1, 2 });

// TODO: Extract class
var players = new List<IPlayer>();
switch (choice)
{
    case 1:
        players.Add(new Player(new GreedyThinker()));
        players.Add(new Player(new GreedyThinker()));
        break;
    case 2:

        var playersToAdd = new ConsoleInputService().GetPlayerInputAsInt("How many players do you want to play (3-7)?", new[] { 3, 4, 5, 6, 7 });

        foreach (var i in Enumerable.Range(3, playersToAdd))
        {
            var name = new ConsoleInputService().GetPlayerInput($"Player number {i - 2} what is your name?");
            players.Add(new Player(new HomoSapiensThinker(new ConsoleInputService()), name));
        }
        break;
}

var dealer = new GameDealer(players);
dealer.Play();

var winner = dealer.Winner();
Console.WriteLine();
Console.WriteLine($"The winner is: {winner.Name}");
Console.WriteLine("And the other players");

foreach (var player in dealer.State.Players)
{
    Console.WriteLine($"- {player}");
}
