using Application.Interfaces;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive;

public class TakeFiveGame : GameOption, IGame<ITakeFivePlayer>
{
    private readonly ITakeFiveGameEventCollector gameEventCollector;
    private TakeFiveGameDealer _gameDealer;

    public TakeFiveGame(ITakeFiveGameEventCollector gameEventCollector) : 
        base(typeof(TakeFiveGame), TakeFiveRules.MinNumberOfPlayers, TakeFiveRules.MaxNumberOfPlayers)
    {
        this.gameEventCollector = gameEventCollector;
    }

    public static string Name { get; } = "TakeFive";

    // REFACTOR - this whole class is duplicate of GesjaaktGame, only few differences are 1. Name (which is just a marker..), 2. Rules 3. eventCollector
    // and it pulls in the correct dependencies to make our life easier.., maybe we can create ABC to avoid DRY?
    public void PlayWith(IEnumerable<ITakeFivePlayer> players)
    {
        // REFACTOR - DI/factory (or is this class the factory?)
        var cardFactory = new TakeFiveCardFactory();
        var deckFactory = new TakeFiveDeckFactory(cardFactory);
        var gameState = new TakeFiveGameState(deckFactory);
        _gameDealer = new TakeFiveGameDealer(gameState);

        _gameDealer.Add(players);

        gameEventCollector
            .Attach(gameState)
            .Attach(_gameDealer)
            .Attach(players);

        _gameDealer.Prepare();
        _gameDealer.Play();
    }

    public IOrderedEnumerable<ITakeFivePlayer> Results()
    {
        return _gameDealer.GetPlayerResults();
    }
}
