using Application.Gesjaakt;
using Domain.Interfaces.Games.Gesjaakt;

namespace Application.Interfaces;

public interface IGesjaaktGameEventCollector
{
    public IGesjaaktGameEventCollector Attach(IGesjaaktGameState gameState);
    public IGesjaaktGameEventCollector Attach(IGesjaaktGameDealer gamedealer);
}