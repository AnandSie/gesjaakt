using Application.Interfaces;
using Domain.Entities.Events;
using Domain.Interfaces.Games.BaseGame;

namespace Application;

public class GameRunnerEventCollector(IGameEventHandler gameEventHandler): IGameRunnerEventCollector
{

    public IGameRunnerEventCollector Attach(IGameRunner gameRunner)
    {
        throw new NotImplementedException();
        //gameState.CardDrawnFromDeck += gameEventHandler.HandleEvent;
        return this;
    }

}
