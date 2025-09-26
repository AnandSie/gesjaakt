using Domain.Entities.Components;
using Domain.Interfaces.Games.BaseGame;

namespace Domain.Entities.Game.BaseGame;

public class CardFactory : ICardFactory<Card>
{
    public Card Create(int value) => new(value);
}
