using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;
using System.Text;

namespace Domain.Entities.Game.Gesjaakt.Thinkers;

public class ManualGesjaaktThinker : IGesjaaktThinker
{
    readonly IPlayerInputProvider _playerInputProvider;
    readonly ILogger<ManualGesjaaktThinker> _logger;
    string _name;

    public ManualGesjaaktThinker(IPlayerInputProvider playerInputProvider, ILogger<ManualGesjaaktThinker> logger, string name)
    {
        _playerInputProvider = playerInputProvider;
        _name = name;
        // TODO: no logger in Domain Entitie
        _logger = logger;
    }

    public GesjaaktTurnOption Decide(IGesjaaktReadOnlyGameState gameState)
    {
        var logMessage = new StringBuilder();

        logMessage.AppendLine("---GAME STATE---");
        logMessage.AppendLine(gameState.ToString());
        logMessage.AppendLine();
        logMessage.AppendLine($"Hi {_name}, what do you want to do?");
        logMessage.AppendLine("1. Take Card  2. Play Coin");

        _logger.LogCritical(logMessage.ToString());

        // TODO: create new method, instead of giving ints, give Enums
        var choice = _playerInputProvider.GetPlayerInputAsInt([1, 2]);
        return choice switch
        {
            1 => GesjaaktTurnOption.TAKECARD,
            2 => GesjaaktTurnOption.SKIPWITHCOIN,
            _ => throw new Exception("Incorrect choice"),
        };
    }

}
