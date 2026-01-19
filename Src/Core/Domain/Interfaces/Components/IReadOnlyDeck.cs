namespace Domain.Interfaces.Components;

public interface IReadOnlyDeck<TCard> where TCard : ICard
{
    int AmountOfCardsLeft();
    bool IsEmpty();
}
