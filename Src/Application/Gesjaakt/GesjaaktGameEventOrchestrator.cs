using Application.Interfaces;
using Domain.Entities.Events;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GesjaaktGameEventOrchestrator(ILogger<GesjaaktGameEventOrchestrator> logger)
{
    public GesjaaktGameEventOrchestrator Attach(IGesjaaktGameState gameState)
    {
        gameState.CardDrawnFromDeck += LogEvent;
        return this;
    }

    public GesjaaktGameEventOrchestrator Attach(IGesjaaktGameDealer gamedealer)
    {
        gamedealer.PlayerGesjaakt += LogEvent;
        gamedealer.SkippedWithCoin += LogEvent;
        gamedealer.CoinsDivided += LogEvent;
        gamedealer.PlayerDecideError += LogEvent;

        return this;
    }

    private void LogEvent(object sender, ErrorEvent eventObject)
    {
        logger.LogError(eventObject.Message);
    }

    private void LogEvent(object sender, WarningEvent eventObject)
    {
        logger.LogWarning(eventObject.Message);
    }

    private void LogEvent(object sender, InfoEvent eventObject)
    {
        logger.LogInformation(eventObject.Message);
    }
}
