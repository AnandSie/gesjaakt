using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Players;

public class HomoSapiensThinker : IThinker
{
    IPlayerInputProvider _playerInputProvider;

    public HomoSapiensThinker(IPlayerInputProvider playerInputProvider)
    {
        _playerInputProvider = playerInputProvider;
    }

    public TurnAction Decide(IGameStateReader gameState)
    {
        Console.Write("-----------\n");
        Console.WriteLine($"Game state is:");
        Console.WriteLine(gameState.ToString());

        Console.Write("");
        Console.WriteLine($"Hai , what do you want to do?");
        Console.WriteLine("1. Take Card 2. Play Coin");

        // TODO: create new method, instead of giving ints, give Enums
        var choice = _playerInputProvider.GetPlayerInputAsInt(new[] { 1, 2 });
        return choice switch
        {
            1 => TurnAction.TAKECARD,
            2 => TurnAction.SKIPWITHCOIN,
            _ => throw new Exception("Incorrect choice"),
        };
    }

}
