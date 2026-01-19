namespace Domain.Interfaces.Components;

public interface IMutableDeck<TCard>: IReadOnlyDeck<TCard> where TCard : ICard
{
    void TakeOut(int amount);
    TCard DrawCard();
}
