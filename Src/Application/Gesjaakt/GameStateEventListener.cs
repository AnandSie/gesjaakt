using Domain.Entities.Events;
using Domain.Interfaces;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Gesjaakt;

public class GameStateEventListener
{
    private readonly ILogger<GameStateEventListener> _logger;

    public GameStateEventListener(ILogger<GameStateEventListener> logger)
    {
        _logger = logger;
    }

    public void Subscribe(IGesjaaktGameState gameState)
    {
        gameState.CardDrawnFromDeck += LogInfo;
    }

    // TODO: maak een enkel listner object
    private void LogInfo(object sender, InfoEvent infoEvent)
    {
        _logger.LogInformation(infoEvent.Message);
    }
}
