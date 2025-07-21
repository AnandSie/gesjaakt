using Domain.Interfaces.Components;
using Domain.Interfaces.Games.TakeFive;

namespace Domain.Entities.Game.TakeFive;

public class TakeFiveGameState : ITakeFiveReadOnlyGameState, ITakeFiveMutableGameState
{
    private HashSet<ITakeFivePlayerState> _players = new HashSet<ITakeFivePlayerState>();

    public IEnumerable<ITakeFivePlayerState> Players => _players;

    public IDeck Deck => throw new NotImplementedException();

    public IEnumerable<IEnumerable<ICard>> CardRows => throw new NotImplementedException();

    public void AddPlayer(ITakeFivePlayerState player)
    {
        _players.Add(player);
    }

    public void InitializeRowsFromDeck()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ICard> PlaceCard(ICard card, int rowNumber)
    {
        throw new NotImplementedException();
    }
}
