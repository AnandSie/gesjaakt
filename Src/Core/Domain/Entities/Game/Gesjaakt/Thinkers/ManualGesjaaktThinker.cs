using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;
using System.Text;

namespace Domain.Entities.Game.Gesjaakt.Thinkers;

public class ManualGesjaaktThinker(IPlayerInputProvider playerInputProvider, string name) : IGesjaaktThinker
{
    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        var logMessage = new StringBuilder();

        logMessage.AppendLine("---GAME STATE---");
        logMessage.AppendLine(gameState.ToString());
        logMessage.AppendLine();
        logMessage.AppendLine($"Hi {name}, what do you want to do?");
        logMessage.AppendLine("1. Take Card  2. Play Coin");

        // REFACTOR: create new method, instead of giving ints, give Enums
        var choice = playerInputProvider.GetPlayerInputAsInt(logMessage.ToString(),[1, 2]);
        return choice switch
        {
            1 => GesjaaktTurnOption.TAKECARD,
            2 => GesjaaktTurnOption.SKIPWITHCOIN,
            _ => throw new Exception("Incorrect choice"),
        };
    }
}
