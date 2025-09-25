using Domain.Entities.Events;
using Domain.Interfaces.Games.Gesjaakt;
using Domain.Interfaces;

namespace Application.Gesjaakt;

public class GameDealerEventListener
{
    private readonly ILogger<GameDealerEventListener> _logger;

    public GameDealerEventListener(ILogger<GameDealerEventListener> logger)
    {
        _logger = logger;
    }

    public void Subscribe(IGesjaaktGameDealer gamedealer)
    {
        gamedealer.PlayerGesjaakt += LogEvent;
        gamedealer.SkippedWithCoin += LogEvent;
        gamedealer.CoinsDivided += LogEvent;
    }

    private void LogEvent(object sender, WarningEvent eventObject)
    {
        _logger.LogWarning(eventObject.Message);
    }

    private void LogEvent(object sender, InfoEvent eventObject)
    {
        _logger.LogInformation(eventObject.Message);
    }
}
