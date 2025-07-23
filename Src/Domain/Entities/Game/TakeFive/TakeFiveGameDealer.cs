using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.Gesjaakt;
using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveGameDealer : IGameDealer<ITakeFivePlayerState>
{
    private readonly ITakeFiveMutableGameState _gameState;

    public TakeFiveGameDealer(ITakeFiveMutableGameState gameState)
    {
        _gameState = gameState;
    }

    public IOrderedEnumerable<ITakeFivePlayerState> GetPlayerResults()
    {
        throw new NotImplementedException();
    }

    public void Play()
    {
        throw new NotImplementedException();
    }

    public void Prepare()
    {
        _gameState.InitializeRowsFromDeck();
        var startAmount = 10; // TODO: Config
        _gameState.DealStartingCards(startAmount);
    }

    public ITakeFivePlayerState Winner()
    {
        throw new NotImplementedException();
    }
}
