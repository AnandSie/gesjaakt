using Application.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GesjaaktGameEventCollector(IGameEventHandler gameEventHandler): IGesjaaktGameEventCollector
{
    public IGesjaaktGameEventCollector Attach(IGesjaaktGameState gameState)
    {
        gameState.CardDrawnFromDeck += gameEventHandler.HandleEvent;
        return this;
    }

    public IGesjaaktGameEventCollector Attach(IGesjaaktGameDealer gamedealer)
    {
        gamedealer.PlayerGesjaakt += gameEventHandler.HandleEvent;
        gamedealer.SkippedWithCoin += gameEventHandler.HandleEvent;
        gamedealer.CoinsDivided += gameEventHandler.HandleEvent;
        gamedealer.PlayerDecideError += gameEventHandler.HandleEvent;
        return this;
    }
}
