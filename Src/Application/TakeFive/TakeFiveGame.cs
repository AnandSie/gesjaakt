using Domain.Interfaces;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.TakeFive;
using System.Text;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveGame : IGame<ITakeFivePlayer>
{
    readonly ILogger<TakeFiveGame> _logger;
    readonly IPlayerInputProvider _playerInputProvider;
    readonly IPlayerFactory<TakeFivePlayer> _playerFactory;
    readonly IGameDealer<ITakeFivePlayer> _gameDealer;

    public TakeFiveGame(ILogger<TakeFiveGame> logger,
                IPlayerInputProvider playerInputProvider,
                IPlayerFactory<TakeFivePlayer> playerFactory,
                IGameDealer<ITakeFivePlayer> gameDealer
        )
    {
        _logger = logger;
        _playerInputProvider = playerInputProvider;
        _playerFactory = playerFactory;
        _gameDealer = gameDealer;
    }

    public void PlayWith(IEnumerable<ITakeFivePlayer> players)
    {
        throw new NotImplementedException();
    }

    public IOrderedEnumerable<ITakeFivePlayer> Results()
    {
        throw new NotImplementedException();
    }

    public void ShowStatistics()
    {
        throw new NotImplementedException();
    }

    public void Simulate()
    {
        throw new NotImplementedException();
    }

    public void SimulateAllPossiblePlayerCombinations()
    {
        throw new NotImplementedException();
    }
}
