using Domain.Interfaces.Components;

namespace Domain.Interfaces.Games.BaseGame;

public interface IDeckFactory<TCard> where TCard : ICard
{
    public IMutableDeck<TCard> Create();
}
