using Application.Interfaces;
using Domain.Entities.Game.TakeFive;
using Domain.Interfaces.Games.BaseGame;
using Domain.Interfaces.Games.TakeFive;

namespace Application.TakeFive;

public class TakeFiveGame(ITakeFiveGameEventCollector gameEventCollector) : IGame<ITakeFivePlayer>
{
    private TakeFiveGameDealer _gameDealer;

    public static string Name { get; } = "TakeFive";

    // REFACTOR - this whole class is duplicate of GesjaaktGame, only two differences - Name (which is just a marker..). This class pulls in the correct dependencies to make our life easier.., maybe we can create ABC to avoid DRY?
    public void PlayWith(IEnumerable<ITakeFivePlayer> players)
    {
        // REFACTOR - DI/factory (or is this class the factory?)
        var cardFactory = new TakeFiveCardFactory();
        var deckFactory = new TakeFiveDeckFactory(cardFactory);
        var gameState = new TakeFiveGameState(deckFactory);
        _gameDealer = new TakeFiveGameDealer(gameState);

        _gameDealer.Add(players);

        // TODO: add
        gameEventCollector
        //    .Attach(gameState) // TODO
        //    .Attach(_gameDealer); // TODO
        .Attach(players);

        _gameDealer.Prepare();
        _gameDealer.Play();
    }

    public IOrderedEnumerable<ITakeFivePlayer> Results()
    {
        return _gameDealer.GetPlayerResults();
    }
}
