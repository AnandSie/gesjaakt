using Domain.Entities.Players;
using Domain.Interfaces;

namespace Domain.Entities.Thinkers;

public class RubenTHinker : IThinker
{
    public TurnAction Decide(IGameStateReader gameState)
    {
        ICollection<ICard> cards = gameState.PlayerOnTurn.Cards;
        int[] cardArray = cards.Select(card => card.Value).ToArray();
        int coinsOnTable = gameState.AmountOfCoinsOnTable;
        int currentCardValue = gameState.OpenCardValue;
        int counsInPocket = gameState.PlayerOnTurn.CoinsAmount;
        double startDeckAmount = 24;
        double coinsWeight = 2.2;
        double cardsLeft = gameState.Deck.AmountOfCardsLeft();
        coinsWeight -= (startDeckAmount - cardsLeft) / startDeckAmount;
        coinsWeight -= gameState.PlayerOnTurn.CoinsAmount / 15;
        double coinsOnTableValue = coinsOnTable * coinsWeight;
        int dealValue = -coinsOnTable;

        if (!IsSequence(currentCardValue, cards))
        {
            dealValue += currentCardValue;
        }

        if (dealValue < 4)
        {
            return TurnAction.TAKECARD;
        }
        else if (coinsOnTable < 2)
        {
            return TurnAction.SKIPWITHCOIN;
        }
        else if (coinsOnTableValue > dealValue)
        {
            return TurnAction.TAKECARD;
        }
        else
        {
            return TurnAction.SKIPWITHCOIN;
        }
    }
    private static bool IsSequence(int currentCardValue, ICollection<ICard> cards)
    {
        foreach (var card in cards)
        {
            if ((currentCardValue == card.Value - 1) || (currentCardValue == card.Value + 1))
            {
                return true;
            }
        }
        return false;
    }

}