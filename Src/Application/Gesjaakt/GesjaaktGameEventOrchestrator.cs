using Domain.Entities.Events;
using Domain.Interfaces;
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

        return this;
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
